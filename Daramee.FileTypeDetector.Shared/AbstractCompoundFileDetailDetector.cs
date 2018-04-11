using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMcdf;

namespace Daramee.FileTypeDetector
{
	public abstract class AbstractCompoundFileDetailDetector : IDetector
	{
		public abstract IEnumerable<string> Chunks { get; }

		public abstract string Extension { get; }
		public virtual string Precondition { get { return null; } }

		protected abstract bool IsValidChunk ( string chunkName, byte [] chunkData );

		public bool Detect ( Stream stream )
		{
			CompoundFile cf = null;

			try
			{
				cf = new CompoundFile ( stream, CFSUpdateMode.ReadOnly, CFSConfiguration.LeaveOpen | CFSConfiguration.Default );

				foreach ( var chunk in Chunks )
				{
					var compoundFileStream = cf.RootStorage.GetStream ( chunk );
					if ( compoundFileStream == null || !IsValidChunk ( chunk, compoundFileStream.GetData () ) )
					{
						//cf.Close ();
						return false;
					}
				}

				cf.Dispose ();

				return true;
			}
			catch
			{
				//if ( cf != null )
					//cf.Close ();
				return false;
			}
		}

		#region Modified OpenMCDF

		// ------------------------------------------------------------- 
		// This is a porting from java code, under MIT license of       |
		// the beautiful Red-Black Tree implementation you can find at  |
		// http://en.literateprograms.org/Red-black_tree_(Java)#chunk   |
		// Many Thanks to original Implementors.                        |
		// -------------------------------------------------------------

		internal class RBTreeException : Exception
		{
			public RBTreeException ( String msg ) : base ( msg ) { }
		}
		internal class RBTreeDuplicatedItemException : RBTreeException
		{
			public RBTreeDuplicatedItemException ( String msg ) : base ( msg ) { }
		}

		internal enum Color { RED = 0, BLACK = 1 }

		internal enum NodeOperation
		{
			LeftAssigned, RightAssigned, ColorAssigned, ParentAssigned,
			ValueAssigned
		}

		internal interface IRBNode : IComparable
		{
			IRBNode Left { get; set; }
			IRBNode Right { get; set; }
			Color Color { get; set; }
			IRBNode Parent { get; set; }
			IRBNode Grandparent ();
			IRBNode Sibling ();
			IRBNode Uncle ();
			void AssignValueTo ( IRBNode other );
		}

		internal class RBTree
		{
			public IRBNode Root { get; set; }

			private static Color NodeColor ( IRBNode n ) => n == null ? Color.BLACK : n.Color;

			public RBTree () { }
			public RBTree ( IRBNode root ) { Root = root; }

			private IRBNode LookupNode ( IRBNode template )
			{
				IRBNode n = Root;
				while ( n != null )
				{
					int compResult = template.CompareTo ( n );
					if ( compResult == 0 ) return n;
					else if ( compResult < 0 ) n = n.Left;
					else n = n.Right;
				}

				return n;
			}

			public bool TryLookup ( IRBNode template, out IRBNode val )
			{
				IRBNode n = LookupNode ( template );

				if ( n == null )
				{
					val = null;
					return false;
				}
				else
				{
					val = n;
					return true;
				}
			}

			private void ReplaceNode ( IRBNode oldn, IRBNode newn )
			{
				if ( oldn.Parent == null ) Root = newn;
				else
				{
					if ( oldn == oldn.Parent.Left )
						oldn.Parent.Left = newn;
					else oldn.Parent.Right = newn;
				}
				if ( newn != null ) newn.Parent = oldn.Parent;
			}

			private void RotateLeft ( IRBNode n )
			{
				IRBNode r = n.Right;
				ReplaceNode ( n, r );
				n.Right = r.Left;
				if ( r.Left != null ) r.Left.Parent = n;
				r.Left = n;
				n.Parent = r;
			}

			private void RotateRight ( IRBNode n )
			{
				IRBNode l = n.Left;
				ReplaceNode ( n, l );
				n.Left = l.Right;
				if ( l.Right != null ) l.Right.Parent = n;
				l.Right = n;
				n.Parent = l;
			}

			public void Insert ( IRBNode newNode )
			{
				newNode.Color = Color.RED;
				IRBNode insertedNode = newNode;

				if ( Root == null ) Root = insertedNode;
				else
				{
					IRBNode n = Root;
					while ( true )
					{
						int compResult = newNode.CompareTo ( n );
						if ( compResult == 0 ) throw new RBTreeDuplicatedItemException ( $"RBNode {newNode} already present in tree" );
						else if ( compResult < 0 )
						{
							if ( n.Left == null )
							{
								n.Left = insertedNode;
								break;
							}
							else n = n.Left;
						}
						else
						{
							if ( n.Right == null )
							{
								n.Right = insertedNode;
								break;
							}
							else n = n.Right;
						}
					}
					insertedNode.Parent = n;
				}

				InsertCase1 ( insertedNode );

				NodeInserted?.Invoke ( insertedNode );
			}

			private void InsertCase1 ( IRBNode n )
			{
				if ( n.Parent == null ) n.Color = Color.BLACK;
				else InsertCase2 ( n );
			}
			private void InsertCase2 ( IRBNode n )
			{
				if ( NodeColor ( n.Parent ) == Color.BLACK ) return;
				else InsertCase3 ( n );
			}
			private void InsertCase3 ( IRBNode n )
			{
				if ( NodeColor ( n.Uncle () ) == Color.RED )
				{
					n.Parent.Color = Color.BLACK;
					n.Uncle ().Color = Color.BLACK;
					n.Grandparent ().Color = Color.RED;
					InsertCase1 ( n.Grandparent () );
				}
				else InsertCase4 ( n );
			}
			private void InsertCase4 ( IRBNode n )
			{
				if ( n == n.Parent.Right && n.Parent == n.Grandparent ().Left )
				{
					RotateLeft ( n.Parent );
					n = n.Left;
				}
				else if ( n == n.Parent.Left && n.Parent == n.Grandparent ().Right )
				{
					RotateRight ( n.Parent );
					n = n.Right;
				}
				InsertCase5 ( n );
			}
			private void InsertCase5 ( IRBNode n )
			{
				n.Parent.Color = Color.BLACK;
				n.Grandparent ().Color = Color.RED;
				if ( n == n.Parent.Left && n.Parent == n.Grandparent ().Left )
					RotateRight ( n.Grandparent () );
				else RotateLeft ( n.Grandparent () );
			}

			private static IRBNode MaximumNode ( IRBNode n )
			{
				while ( n.Right != null )
					n = n.Right;
				return n;
			}

			public void Delete ( IRBNode template, out IRBNode deletedAlt )
			{
				deletedAlt = null;
				IRBNode n = LookupNode ( template );
				template = n;
				if ( n == null ) return;
				if ( n.Left != null && n.Right != null )
				{
					IRBNode pred = MaximumNode ( n.Left );
					pred.AssignValueTo ( n );
					n = pred;
					deletedAlt = pred;
				}

				IRBNode child = n.Right ?? n.Left;
				if ( NodeColor ( n ) == Color.BLACK )
				{
					n.Color = NodeColor ( child );
					DeleteCase1 ( n );
				}

				ReplaceNode ( n, child );

				if ( NodeColor ( Root ) == Color.RED )
					Root.Color = Color.BLACK;

				return;
			}

			private void DeleteCase1 ( IRBNode n )
			{
				if ( n.Parent == null ) return;
				else DeleteCase2 ( n );
			}

			private void DeleteCase2 ( IRBNode n )
			{
				if ( NodeColor ( n.Sibling () ) == Color.RED )
				{
					n.Parent.Color = Color.RED;
					n.Sibling ().Color = Color.BLACK;
					if ( n == n.Parent.Left )
						RotateLeft ( n.Parent );
					else
						RotateRight ( n.Parent );
				}

				DeleteCase3 ( n );
			}

			private void DeleteCase3 ( IRBNode n )
			{
				if ( NodeColor ( n.Parent ) == Color.BLACK &&
					NodeColor ( n.Sibling () ) == Color.BLACK &&
					NodeColor ( n.Sibling ().Left ) == Color.BLACK &&
					NodeColor ( n.Sibling ().Right ) == Color.BLACK )
				{
					n.Sibling ().Color = Color.RED;
					DeleteCase1 ( n.Parent );
				}
				else DeleteCase4 ( n );
			}

			private void DeleteCase4 ( IRBNode n )
			{
				if ( NodeColor ( n.Parent ) == Color.RED &&
					NodeColor ( n.Sibling () ) == Color.BLACK &&
					NodeColor ( n.Sibling ().Left ) == Color.BLACK &&
					NodeColor ( n.Sibling ().Right ) == Color.BLACK )
				{
					n.Sibling ().Color = Color.RED;
					n.Parent.Color = Color.BLACK;
				}
				else DeleteCase5 ( n );
			}

			private void DeleteCase5 ( IRBNode n )
			{
				if ( n == n.Parent.Left &&
					NodeColor ( n.Sibling () ) == Color.BLACK &&
					NodeColor ( n.Sibling ().Left ) == Color.RED &&
					NodeColor ( n.Sibling ().Right ) == Color.BLACK )
				{
					n.Sibling ().Color = Color.RED;
					n.Sibling ().Left.Color = Color.BLACK;
					RotateRight ( n.Sibling () );
				}
				else if ( n == n.Parent.Right &&
						 NodeColor ( n.Sibling () ) == Color.BLACK &&
						 NodeColor ( n.Sibling ().Right ) == Color.RED &&
						 NodeColor ( n.Sibling ().Left ) == Color.BLACK )
				{
					n.Sibling ().Color = Color.RED;
					n.Sibling ().Right.Color = Color.BLACK;
					RotateLeft ( n.Sibling () );
				}

				DeleteCase6 ( n );
			}

			private void DeleteCase6 ( IRBNode n )
			{
				n.Sibling ().Color = NodeColor ( n.Parent );
				n.Parent.Color = Color.BLACK;
				if ( n == n.Parent.Left )
				{
					n.Sibling ().Right.Color = Color.BLACK;
					RotateLeft ( n.Parent );
				}
				else
				{
					n.Sibling ().Left.Color = Color.BLACK;
					RotateRight ( n.Parent );
				}
			}

			public void VisitTree ( Action<IRBNode> action )
			{
				IRBNode walker = Root;
				if ( walker != null )
					DoVisitTree ( action, walker );
			}

			private void DoVisitTree ( Action<IRBNode> action, IRBNode walker )
			{
				if ( walker.Left != null )
					DoVisitTree ( action, walker.Left );

				action?.Invoke ( walker );

				if ( walker.Right != null )
					DoVisitTree ( action, walker.Right );
			}

			internal void VisitTreeNodes ( Action<IRBNode> action )
			{
				IRBNode walker = Root;
				if ( walker != null )
					DoVisitTreeNodes ( action, walker );
			}

			private void DoVisitTreeNodes ( Action<IRBNode> action, IRBNode walker )
			{
				if ( walker.Left != null )
					DoVisitTreeNodes ( action, walker.Left );

				action?.Invoke ( walker );

				if ( walker.Right != null )
					DoVisitTreeNodes ( action, walker.Right );
			}

			public class RBTreeEnumerator : IEnumerator<IRBNode>
			{
				int position = -1;
				private Queue<IRBNode> heap = new Queue<IRBNode> ();

				internal RBTreeEnumerator ( RBTree tree ) { tree.VisitTreeNodes ( item => heap.Enqueue ( item ) ); }

				public IRBNode Current => heap.ElementAt ( position );

				public void Dispose () { }

				object System.Collections.IEnumerator.Current => heap.ElementAt ( position );

				public bool MoveNext () => ( ++position < heap.Count );

				public void Reset () { position = -1; }
			}

			public RBTreeEnumerator GetEnumerator () => new RBTreeEnumerator ( this );

			private static int INDENT_STEP = 15;

			public void Print () { PrintHelper ( Root, 0 ); }

			private static void PrintHelper ( IRBNode n, int indent )
			{
				if ( n == null )
					return;

				if ( n.Left != null ) PrintHelper ( n.Left, indent + INDENT_STEP );
				if ( n.Right != null ) PrintHelper ( n.Right, indent + INDENT_STEP );
			}

			internal void FireNodeOperation ( IRBNode node, NodeOperation operation )
			{
				NodeOperation?.Invoke ( node, operation );
			}

			internal event Action<IRBNode> NodeInserted;
			internal event Action<IRBNode, NodeOperation> NodeOperation;
		}

		/* This Source Code Form is subject to the terms of the Mozilla Public
		 * License, v. 2.0. If a copy of the MPL was not distributed with this
		 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
		 * 
		 * The Original Code is OpenMCDF - Compound Document Format library.
		 * 
		 * The Initial Developer of the Original Code is Federico Blaseotto.*/

		internal class CFException : Exception
		{
			public CFException () : base () { }
			public CFException ( string message ) : base ( message, null ) { }
			public CFException ( string message, Exception innerException ) : base ( message, innerException ) { }
		}

		internal class CFDisposedException : CFException
		{
			public CFDisposedException () : base () { }
			public CFDisposedException ( string message ) : base ( message, null ) { }
			public CFDisposedException ( string message, Exception innerException ) : base ( message, innerException ) { }
		}

		internal class CFFileFormatException : CFException
		{
			public CFFileFormatException () : base () { }
			public CFFileFormatException ( string message ) : base ( message, null ) { }
			public CFFileFormatException ( string message, Exception innerException ) : base ( message, innerException ) { }
		}

		internal class CFItemNotFound : CFException
		{
			public CFItemNotFound () : base ( "Entry not found" ) { }
			public CFItemNotFound ( string message ) : base ( message, null ) { }
			public CFItemNotFound ( string message, Exception innerException ) : base ( message, innerException ) { }
		}

		internal class CFInvalidOperation : CFException
		{
			public CFInvalidOperation () : base () { }
			public CFInvalidOperation ( string message ) : base ( message, null ) { }
			public CFInvalidOperation ( string message, Exception innerException ) : base ( message, innerException ) { }
		}

		internal class CFDuplicatedItemException : CFException
		{
			public CFDuplicatedItemException () : base () { }
			public CFDuplicatedItemException ( string message ) : base ( message, null ) { }
			public CFDuplicatedItemException ( string message, Exception innerException ) : base ( message, innerException ) { }
		}

		internal class CFCorruptedFileException : CFException
		{
			public CFCorruptedFileException () : base () { }
			public CFCorruptedFileException ( string message ) : base ( message, null ) { }
			public CFCorruptedFileException ( string message, Exception innerException ) : base ( message, innerException ) { }
		}

		internal enum SectorType
		{
			Normal, Mini, FAT, DIFAT, RangeLockSector, Directory
		}

		internal class Sector : IDisposable
		{
			public static int MINISECTOR_SIZE = 64;

			public const int FREESECT = unchecked(( int ) 0xFFFFFFFF);
			public const int ENDOFCHAIN = unchecked(( int ) 0xFFFFFFFE);
			public const int FATSECT = unchecked(( int ) 0xFFFFFFFD);
			public const int DIFSECT = unchecked(( int ) 0xFFFFFFFC);

			public bool DirtyFlag { get; set; }

			public bool IsStreamed => ( stream != null && Size != MINISECTOR_SIZE ) ? ( Id * Size ) + Size < stream.Length : false;

			private Stream stream;

			public Sector ( int size, Stream stream )
			{
				this.Size = size;
				this.stream = stream;
			}

			public Sector ( int size, byte [] data )
			{
				this.Size = size;
				this.data = data;
				this.stream = null;
			}

			public Sector ( int size )
			{
				this.Size = size;
				this.data = null;
				this.stream = null;
			}

			internal SectorType Type { get; set; }

			public int Id { get; set; } = -1;
			public int Size { get; private set; } = 0;

			private byte [] data;
			public byte [] GetData ()
			{
				if ( data == null )
				{
					data = new byte [ Size ];

					if ( IsStreamed )
					{
						stream.Seek ( ( long ) Size + ( long ) this.Id * ( long ) Size, SeekOrigin.Begin );
						stream.Read ( data, 0, Size );
					}
				}

				return data;
			}

			public void ZeroData ()
			{
				data = new byte [ Size ];
				DirtyFlag = true;
			}

			public void InitFATData ()
			{
				data = new byte [ Size ];

				for ( int i = 0; i < Size; i++ )
					data [ i ] = 0xFF;

				DirtyFlag = true;
			}

			internal void ReleaseData ()
			{
				this.data = null;
			}

			private object lockObject = new Object ();

			protected virtual void Dispose ( bool disposing )
			{
				try
				{
					if ( !_disposed )
					{
						lock ( lockObject )
						{
							this.data = null;
							this.DirtyFlag = false;
							this.Id = Sector.ENDOFCHAIN;
							this.Size = 0;
						}
					}
				}
				finally { _disposed = true; }
			}

			#region IDisposable Members

			private bool _disposed;//false

			void IDisposable.Dispose ()
			{
				Dispose ( true );
				GC.SuppressFinalize ( this );
			}

			#endregion
		}

		internal delegate void Ver3SizeLimitReached ();

		internal class SectorCollection : IList<Sector>
		{
			private const int MAX_SECTOR_V4_COUNT_LOCK_RANGE = 524287; //0x7FFFFF00 for Version 4
			private const int SLICE_SIZE = 4096;

			public event Ver3SizeLimitReached OnVer3SizeLimitReached;

			private List<List<object>> largeArraySlices = new List<List<object>> ();

			public SectorCollection () { }

			private bool sizeLimitReached = false;
			private void DoCheckSizeLimitReached ()
			{
				if ( !sizeLimitReached && ( Count - 1 > MAX_SECTOR_V4_COUNT_LOCK_RANGE ) )
				{
					OnVer3SizeLimitReached?.Invoke ();
					sizeLimitReached = true;
				}
			}

			#region IList<T> Members

			public int IndexOf ( Sector item ) { throw new NotImplementedException (); }
			public void Insert ( int index, Sector item ) { throw new NotImplementedException (); }
			public void RemoveAt ( int index ) { throw new NotImplementedException (); }

			public Sector this [ int index ]
			{
				get
				{
					int itemIndex = index / SLICE_SIZE;
					int itemOffset = index % SLICE_SIZE;

					if ( ( index > -1 ) && ( index < Count ) )
						return ( Sector ) largeArraySlices [ itemIndex ] [ itemOffset ];
					else
						throw new ArgumentOutOfRangeException ( "index", index, "Argument out of range" );
				}
				set
				{
					int itemIndex = index / SLICE_SIZE;
					int itemOffset = index % SLICE_SIZE;

					if ( index > -1 && index < Count )
						largeArraySlices [ itemIndex ] [ itemOffset ] = value;
					else
						throw new ArgumentOutOfRangeException ( "index", index, "Argument out of range" );
				}
			}

			#endregion

			#region ICollection<T> Members

			private int InternalAdd ( Sector item )
			{
				int itemIndex = Count / SLICE_SIZE;

				if ( itemIndex < largeArraySlices.Count )
				{
					largeArraySlices [ itemIndex ].Add ( item );
					Count++;
				}
				else
				{
					largeArraySlices.Add ( new List<object> ( SLICE_SIZE ) { item } );
					++Count;
				}

				return Count - 1;
			}

			public void Add ( Sector item )
			{
				DoCheckSizeLimitReached ();
				InternalAdd ( item );

			}

			public void Clear ()
			{
				foreach ( List<object> slice in largeArraySlices )
					slice.Clear ();
				largeArraySlices.Clear ();
				Count = 0;
			}

			public bool Contains ( Sector item ) { throw new NotImplementedException (); }

			public void CopyTo ( Sector [] array, int arrayIndex ) { throw new NotImplementedException (); }

			public int Count { get; private set; }

			public bool IsReadOnly => false;

			public bool Remove ( Sector item ) { throw new NotImplementedException (); }

			#endregion

			#region IEnumerable<T> Members

			public IEnumerator<Sector> GetEnumerator ()
			{
				for ( int i = 0; i < largeArraySlices.Count; i++ )
					for ( int j = 0; j < largeArraySlices [ i ].Count; j++ )
						yield return ( Sector ) largeArraySlices [ i ] [ j ];
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
			{
				for ( int i = 0; i < largeArraySlices.Count; i++ )
					for ( int j = 0; j < largeArraySlices [ i ].Count; j++ )
						yield return largeArraySlices [ i ] [ j ];
			}

			#endregion
		}

		internal enum StgType : int
		{
			StgInvalid = 0,
			StgStorage = 1,
			StgStream = 2,
			StgLockbytes = 3,
			StgProperty = 4,
			StgRoot = 5
		}

		internal enum StgColor : int
		{
			Red = 0,
			Black = 1
		}

		internal interface IDirectoryEntry : IComparable, IRBNode
		{
			int Child { get; set; }
			byte [] CreationDate { get; set; }
			byte [] EntryName { get; }
			string GetEntryName ();
			int LeftSibling { get; set; }
			byte [] ModifyDate { get; set; }
			string Name { get; }
			ushort NameLength { get; set; }
			void Read ( System.IO.Stream stream, CFSVersion ver = CFSVersion.Ver_3 );
			int RightSibling { get; set; }
			void SetEntryName ( string entryName );
			int SID { get; set; }
			long Size { get; set; }
			int StartSetc { get; set; }
			int StateBits { get; set; }
			StgColor StgColor { get; set; }
			StgType StgType { get; set; }
			Guid StorageCLSID { get; set; }
		}

		internal class DirectoryEntry : IDirectoryEntry
		{
			internal const int THIS_IS_GREATER = 1;
			internal const int OTHER_IS_GREATER = -1;
			private IList<IDirectoryEntry> dirRepository;

			public int SID { get; set; } = -1;

			internal static Int32 NOSTREAM
				= unchecked(( int ) 0xFFFFFFFF);

			private DirectoryEntry ( String name, StgType stgType, IList<IDirectoryEntry> dirRepository )
			{
				this.dirRepository = dirRepository;

				this.stgType = stgType;

				switch ( stgType )
				{
					case StgType.StgStream:

						storageCLSID = new Guid ( "00000000000000000000000000000000" );
						creationDate = new byte [ 8 ] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
						modifyDate = new byte [ 8 ] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
						break;

					case StgType.StgStorage:
						creationDate = BitConverter.GetBytes ( ( DateTime.Now.ToFileTime () ) );
						break;

					case StgType.StgRoot:
						creationDate = new byte [ 8 ] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
						modifyDate = new byte [ 8 ] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
						break;
				}

				this.SetEntryName ( name );

			}

			public byte [] EntryName { get; private set; } = new byte [ 64 ];

			public String GetEntryName ()
			{
				if ( EntryName != null && EntryName.Length > 0 )
				{
					return Encoding.Unicode.GetString ( EntryName ).Remove ( ( nameLength - 1 ) / 2 );
				}
				else
					return String.Empty;
			}

			public void SetEntryName ( String entryName )
			{
				if (
					entryName.Contains ( @"\" ) ||
					entryName.Contains ( @"/" ) ||
					entryName.Contains ( @":" ) ||
					entryName.Contains ( @"!" )

					)
					throw new CFException ( "Invalid character in entry: the characters '\\', '/', ':','!' cannot be used in entry name" );

				if ( entryName.Length > 31 )
					throw new CFException ( "Entry name MUST be smaller than 31 characters" );



				byte [] newName = null;
				byte [] temp = Encoding.Unicode.GetBytes ( entryName );
				newName = new byte [ 64 ];
				Buffer.BlockCopy ( temp, 0, newName, 0, temp.Length );
				newName [ temp.Length ] = 0x00;
				newName [ temp.Length + 1 ] = 0x00;

				EntryName = newName;
				nameLength = ( ushort ) ( temp.Length + 2 );

			}

			private ushort nameLength;
			public ushort NameLength
			{
				get
				{
					return nameLength;
				}
				set
				{
					throw new NotImplementedException ();
				}
			}

			private StgType stgType = StgType.StgInvalid;
			public StgType StgType
			{
				get
				{
					return stgType;
				}
				set
				{
					stgType = value;
				}
			}
			private StgColor stgColor = StgColor.Black;

			public StgColor StgColor
			{
				get
				{
					return stgColor;
				}
				set
				{
					stgColor = value;
				}
			}

			private Int32 leftSibling = NOSTREAM;
			public Int32 LeftSibling
			{
				get { return leftSibling; }
				set { leftSibling = value; }
			}

			private Int32 rightSibling = NOSTREAM;
			public Int32 RightSibling
			{
				get { return rightSibling; }
				set { rightSibling = value; }
			}

			private Int32 child = NOSTREAM;
			public Int32 Child
			{
				get { return child; }
				set { child = value; }
			}

			private Guid storageCLSID
				= Guid.NewGuid ();

			public Guid StorageCLSID
			{
				get
				{
					return storageCLSID;
				}
				set
				{
					this.storageCLSID = value;
				}
			}


			private Int32 stateBits;

			public Int32 StateBits
			{
				get { return stateBits; }
				set { stateBits = value; }
			}

			private byte [] creationDate = new byte [ 8 ] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

			public byte [] CreationDate
			{
				get
				{
					return creationDate;
				}
				set
				{
					creationDate = value;
				}
			}

			private byte [] modifyDate = new byte [ 8 ] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

			public byte [] ModifyDate
			{
				get
				{
					return modifyDate;
				}
				set
				{
					modifyDate = value;
				}
			}

			private Int32 startSetc = Sector.ENDOFCHAIN;
			public Int32 StartSetc
			{
				get
				{
					return startSetc;
				}
				set
				{
					startSetc = value;
				}
			}
			private long size;
			public long Size
			{
				get
				{
					return size;
				}
				set
				{
					size = value;
				}
			}


			public int CompareTo ( object obj )
			{

				IDirectoryEntry otherDir = obj as IDirectoryEntry;

				if ( otherDir == null )
					throw new CFException ( "Invalid casting: compared object does not implement IDirectorEntry interface" );

				if ( this.NameLength > otherDir.NameLength )
				{
					return THIS_IS_GREATER;
				}
				else if ( this.NameLength < otherDir.NameLength )
				{
					return OTHER_IS_GREATER;
				}
				else
				{
					String thisName = Encoding.Unicode.GetString ( this.EntryName, 0, this.NameLength );
					String otherName = Encoding.Unicode.GetString ( otherDir.EntryName, 0, otherDir.NameLength );

					for ( int z = 0; z < thisName.Length; z++ )
					{
						char thisChar = char.ToUpperInvariant ( thisName [ z ] );
						char otherChar = char.ToUpperInvariant ( otherName [ z ] );

						if ( thisChar > otherChar )
							return THIS_IS_GREATER;
						else if ( thisChar < otherChar )
							return OTHER_IS_GREATER;
					}

					return 0;

				}

				//   return String.Compare(Encoding.Unicode.GetString(this.EntryName).ToUpper(), Encoding.Unicode.GetString(other.EntryName).ToUpper());
			}

			public override bool Equals ( object obj )
			{
				return CompareTo ( obj ) == 0;
			}

			/// <summary>
			/// FNV hash, short for Fowler/Noll/Vo
			/// </summary>
			/// <param name="buffer"></param>
			/// <returns>(not warranted) unique hash for byte array</returns>
			private static ulong Fnv_hash ( byte [] buffer )
			{

				ulong h = 2166136261;
				int i;

				for ( i = 0; i < buffer.Length; i++ )
					h = ( h * 16777619 ) ^ buffer [ i ];

				return h;
			}

			public override int GetHashCode ()
			{
				return ( int ) Fnv_hash ( EntryName );
			}

			public void Read ( Stream stream, CFSVersion ver = CFSVersion.Ver_3 )
			{
				using ( BinaryReader rw = new BinaryReader ( stream, Encoding.UTF8, true ) )
				{
					EntryName = rw.ReadBytes ( 64 );
					nameLength = rw.ReadUInt16 ();
					stgType = ( StgType ) rw.ReadByte ();
					stgColor = ( StgColor ) rw.ReadByte ();
					leftSibling = rw.ReadInt32 ();
					rightSibling = rw.ReadInt32 ();
					child = rw.ReadInt32 ();

					// Thanks to bugaccount (BugTrack id 3519554)
					if ( stgType == StgType.StgInvalid )
					{
						leftSibling = NOSTREAM;
						rightSibling = NOSTREAM;
						child = NOSTREAM;
					}

					storageCLSID = new Guid ( rw.ReadBytes ( 16 ) );
					stateBits = rw.ReadInt32 ();
					creationDate = rw.ReadBytes ( 8 );
					modifyDate = rw.ReadBytes ( 8 );
					startSetc = rw.ReadInt32 ();

					if ( ver == CFSVersion.Ver_3 )
					{
						size = rw.ReadInt32 ();
						rw.ReadBytes ( 4 ); //discard most significant 4 (possibly) dirty bytes
					}
					else
						size = rw.ReadInt64 ();
				}
			}

			public string Name
			{
				get { return GetEntryName (); }
			}


			public IRBNode Left
			{
				get
				{
					if ( leftSibling == DirectoryEntry.NOSTREAM )
						return null;

					return dirRepository [ leftSibling ];
				}
				set
				{
					leftSibling = value != null ? ( ( IDirectoryEntry ) value ).SID : DirectoryEntry.NOSTREAM;

					if ( leftSibling != DirectoryEntry.NOSTREAM )
						dirRepository [ leftSibling ].Parent = this;
				}
			}

			public IRBNode Right
			{
				get
				{
					if ( rightSibling == DirectoryEntry.NOSTREAM )
						return null;

					return dirRepository [ rightSibling ];
				}
				set
				{

					rightSibling = value != null ? ( ( IDirectoryEntry ) value ).SID : DirectoryEntry.NOSTREAM;

					if ( rightSibling != DirectoryEntry.NOSTREAM )
						dirRepository [ rightSibling ].Parent = this;

				}
			}

			public Color Color
			{
				get
				{
					return ( Color ) StgColor;
				}
				set
				{
					StgColor = ( StgColor ) value;
				}
			}

			private IDirectoryEntry parent = null;

			public IRBNode Parent
			{
				get
				{
					return parent;
				}
				set
				{
					parent = value as IDirectoryEntry;
				}
			}

			public IRBNode Grandparent () => parent?.Parent;
			public IRBNode Sibling () => ( this == Parent.Left ) ? Parent.Right : Parent.Left;
			public IRBNode Uncle () => parent?.Sibling ();

			internal static IDirectoryEntry New ( String name, StgType stgType, IList<IDirectoryEntry> dirRepository )
			{
				DirectoryEntry de = null;
				if ( dirRepository != null )
				{
					de = new DirectoryEntry ( name, stgType, dirRepository );
					// No invalid directory entry found
					dirRepository.Add ( de );
					de.SID = dirRepository.Count - 1;
				}
				else
					throw new ArgumentNullException ( "dirRepository", "Directory repository cannot be null in New() method" );

				return de;
			}

			internal static IDirectoryEntry Mock ( String name, StgType stgType ) => new DirectoryEntry ( name, stgType, null );

			internal static IDirectoryEntry TryNew ( String name, StgType stgType, IList<IDirectoryEntry> dirRepository )
			{
				DirectoryEntry de = new DirectoryEntry ( name, stgType, dirRepository );

				// If we are not adding an invalid dirEntry as
				// in a normal loading from file (invalid dirs MAY pad a sector)
				if ( de != null )
				{
					// Find first available invalid slot (if any) to reuse it
					for ( int i = 0; i < dirRepository.Count; i++ )
					{
						if ( dirRepository [ i ].StgType == StgType.StgInvalid )
						{
							dirRepository [ i ] = de;
							de.SID = i;
							return de;
						}
					}
				}

				// No invalid directory entry found
				dirRepository.Add ( de );
				de.SID = dirRepository.Count - 1;

				return de;
			}

			public override string ToString ()
			{
				return $"{Name} [{SID}]{ ( stgType == StgType.StgStream ? "Stream" : "Storage" ) }";
			}


			public void AssignValueTo ( IRBNode other )
			{
				DirectoryEntry d = other as DirectoryEntry;

				d.SetEntryName ( GetEntryName () );

				d.creationDate = new byte [ creationDate.Length ];
				creationDate.CopyTo ( d.creationDate, 0 );

				d.modifyDate = new byte [ modifyDate.Length ];
				modifyDate.CopyTo ( d.modifyDate, 0 );

				d.size = size;
				d.startSetc = startSetc;
				d.stateBits = stateBits;
				d.stgType = stgType;
				d.storageCLSID = new Guid ( storageCLSID.ToByteArray () );
				d.Child = Child;
			}
		}

		internal class DirectoryEntryCollection : ICollection<IDirectoryEntry>
		{
			private List<IDirectoryEntry> directoryEntries;

			internal DirectoryEntryCollection ( List<IDirectoryEntry> directoryEntries ) { this.directoryEntries = directoryEntries; }

			public bool IsReadOnly => true;
			public int Count => directoryEntries.Count;

			public void Add ( IDirectoryEntry item ) => throw new NotImplementedException ();
			public bool Remove ( IDirectoryEntry item ) => throw new NotImplementedException ();
			public void Clear () => throw new NotImplementedException ();

			public bool Contains ( IDirectoryEntry item ) => directoryEntries.Contains ( item );

			public void CopyTo ( IDirectoryEntry [] array, int arrayIndex ) => throw new NotImplementedException ();

			#region IEnumerable<IDirectoryEntry> Members
			public IEnumerator<IDirectoryEntry> GetEnumerator () => directoryEntries.GetEnumerator ();
			#endregion

			#region IEnumerable Members
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () => directoryEntries.GetEnumerator ();
			#endregion
		}

		internal class Directory
		{
			internal bool IsCompoundFileDisposed { get; set; }
		}

		internal abstract class CFItem : IComparable<CFItem>
		{
			private CompoundFile compoundFile;

			protected CompoundFile CompoundFile => compoundFile;

			protected void CheckDisposed ()
			{
				if ( compoundFile.IsClosed )
					throw new CFDisposedException ( "Owner Compound file has been closed and owned items have been invalidated" );
			}

			protected CFItem () { }
			protected CFItem ( CompoundFile compoundFile ) { this.compoundFile = compoundFile; }

			#region IDirectoryEntry Members
			internal IDirectoryEntry DirEntry { get; set; }
			internal int CompareTo ( CFItem other ) => DirEntry.CompareTo ( other.DirEntry );
			#endregion

			#region IComparable Members
			public int CompareTo ( object obj )
			{
				return DirEntry.CompareTo ( ( obj as CFItem ).DirEntry );
			}
			#endregion

			public static bool operator == ( CFItem leftItem, CFItem rightItem )
			{
				if ( System.Object.ReferenceEquals ( leftItem, rightItem ) )
					return true;
				if ( ( ( object ) leftItem == null ) || ( ( object ) rightItem == null ) )
					return false;
				return leftItem.CompareTo ( rightItem ) == 0;
			}

			public static bool operator != ( CFItem leftItem, CFItem rightItem ) => !( leftItem == rightItem );
			public override bool Equals ( object obj ) => CompareTo ( obj ) == 0;
			public override int GetHashCode () => DirEntry.GetEntryName ().GetHashCode ();

			public string Name
			{
				get
				{
					var n = DirEntry.GetEntryName ();
					return ( n != null && n.Length > 0 ) ? n.TrimEnd ( '\0' ) : string.Empty;
				}
			}

			public long Size => DirEntry.Size;
			public bool IsStorage => DirEntry.StgType == StgType.StgStorage;
			public bool IsStream => DirEntry.StgType == StgType.StgStream;
			public bool IsRoot => DirEntry.StgType == StgType.StgRoot;

			public DateTime CreationDate
			{
				get => DateTime.FromFileTime ( BitConverter.ToInt64 ( DirEntry.CreationDate, 0 ) );
				set
				{
					if ( DirEntry.StgType != StgType.StgStream && DirEntry.StgType != StgType.StgRoot )
						DirEntry.CreationDate = BitConverter.GetBytes ( ( value.ToFileTime () ) );
					else
						throw new CFException ( "Creation Date can only be set on storage entries" );
				}
			}

			public DateTime ModifyDate
			{
				get => DateTime.FromFileTime ( BitConverter.ToInt64 ( DirEntry.ModifyDate, 0 ) );
				set
				{
					if ( DirEntry.StgType != StgType.StgStream && DirEntry.StgType != StgType.StgRoot )
						DirEntry.ModifyDate = BitConverter.GetBytes ( ( value.ToFileTime () ) );
					else
						throw new CFException ( "Modify Date can only be set on storage entries" );
				}
			}

			public Guid CLSID
			{
				get => DirEntry.StorageCLSID;
				set
				{
					if ( DirEntry.StgType != StgType.StgStream )
						DirEntry.StorageCLSID = value;
					else
						throw new CFException ( "Object class GUID can only be set on Root and Storage entries" );
				}
			}

			int IComparable<CFItem>.CompareTo ( CFItem other ) => DirEntry.CompareTo ( other.DirEntry );

			public override string ToString ()
			{
				return ( DirEntry != null )
					? $"[{DirEntry.LeftSibling},{DirEntry.SID},{DirEntry.RightSibling}] {DirEntry.GetEntryName ()}"
					: string.Empty;
			}
		}

		internal delegate void VisitedEntryAction ( CFItem item );

		internal class CFStream : CFItem
		{
			internal CFStream ( CompoundFile compoundFile, IDirectoryEntry dirEntry )
				: base ( compoundFile )
			{
				if ( dirEntry == null || dirEntry.SID < 0 )
					throw new CFException ( "Attempting to add a CFStream using an unitialized directory" );
				this.DirEntry = dirEntry;
			}

			public Byte [] GetData ()
			{
				CheckDisposed ();
				return this.CompoundFile.GetData ( this );
			}

			public int Read ( byte [] buffer, long position, int count )
			{
				CheckDisposed ();
				return this.CompoundFile.ReadData ( this, position, buffer, 0, count );
			}

			internal int Read ( byte [] buffer, long position, int offset, int count )
			{
				CheckDisposed ();
				return this.CompoundFile.ReadData ( this, position, buffer, offset, count );
			}
		}

		internal class CFStorage : CFItem
		{
			private RBTree children;

			internal RBTree Children
			{
				get
				{
					// Lazy loading of children tree.
					if ( children == null )
					{
						children = LoadChildren ( this.DirEntry.SID );
						if ( children == null )
							children = this.CompoundFile.CreateNewTree ();
					}

					return children;
				}
			}

			internal CFStorage ( CompoundFile compFile, IDirectoryEntry dirEntry )
				: base ( compFile )
			{
				if ( dirEntry == null || dirEntry.SID < 0 )
					throw new CFException ( "Attempting to create a CFStorage using an unitialized directory" );

				this.DirEntry = dirEntry;
			}

			private RBTree LoadChildren ( int SID )
			{
				RBTree childrenTree = this.CompoundFile.GetChildrenTree ( SID );

				if ( childrenTree.Root != null )
					this.DirEntry.Child = ( childrenTree.Root as IDirectoryEntry ).SID;
				else
					this.DirEntry.Child = DirectoryEntry.NOSTREAM;

				return childrenTree;
			}

			public CFStream GetStream ( String streamName )
			{
				CheckDisposed ();

				IDirectoryEntry tmp = DirectoryEntry.Mock ( streamName, StgType.StgStream );

				if ( Children.TryLookup ( tmp, out IRBNode outDe ) && ( ( ( IDirectoryEntry ) outDe ).StgType == StgType.StgStream ) )
					return new CFStream ( this.CompoundFile, ( IDirectoryEntry ) outDe );
				else
					throw new CFItemNotFound ( "Cannot find item [" + streamName + "] within the current storage" );
			}

			public CFStream TryGetStream ( String streamName )
			{
				CheckDisposed ();

				IDirectoryEntry tmp = DirectoryEntry.Mock ( streamName, StgType.StgStream );

				if ( Children.TryLookup ( tmp, out IRBNode outDe ) && ( ( ( IDirectoryEntry ) outDe ).StgType == StgType.StgStream ) )
					return new CFStream ( this.CompoundFile, ( IDirectoryEntry ) outDe );
				else
					return null;
			}

			public CFStorage GetStorage ( String storageName )
			{
				CheckDisposed ();

				IDirectoryEntry template = DirectoryEntry.Mock ( storageName, StgType.StgInvalid );

				if ( Children.TryLookup ( template, out IRBNode outDe ) && ( ( IDirectoryEntry ) outDe ).StgType == StgType.StgStorage )
					return new CFStorage ( this.CompoundFile, outDe as IDirectoryEntry );
				else
					throw new CFItemNotFound ( "Cannot find item [" + storageName + "] within the current storage" );
			}

			public CFStorage TryGetStorage ( String storageName )
			{
				CheckDisposed ();

				IDirectoryEntry template = DirectoryEntry.Mock ( storageName, StgType.StgInvalid );

				if ( Children.TryLookup ( template, out IRBNode outDe ) && ( ( IDirectoryEntry ) outDe ).StgType == StgType.StgStorage )
					return new CFStorage ( this.CompoundFile, outDe as IDirectoryEntry );
				else
					return null;
			}

			public void VisitEntries ( Action<CFItem> action, bool recursive )
			{
				CheckDisposed ();

				if ( action != null )
				{
					List<IRBNode> subStorages = new List<IRBNode> ();

					void internalAction ( IRBNode targetNode )
					{
						IDirectoryEntry d = targetNode as IDirectoryEntry;
						if ( d.StgType == StgType.StgStream )
							action ( new CFStream ( this.CompoundFile, d ) );
						else
							action ( new CFStorage ( this.CompoundFile, d ) );

						if ( d.Child != DirectoryEntry.NOSTREAM )
							subStorages.Add ( targetNode );

						return;
					}

					this.Children.VisitTreeNodes ( internalAction );

					if ( recursive && subStorages.Count > 0 )
						foreach ( IRBNode n in subStorages )
							( new CFStorage ( this.CompoundFile, n as IDirectoryEntry ) ).VisitEntries ( action, recursive );
				}
			}
		}

		internal class CFItemComparer : IComparer<CFItem>
		{
			public int Compare ( CFItem x, CFItem y )
			{
				// X CompareTo Y : X > Y --> 1 ; X < Y  --> -1
				return ( x.DirEntry.CompareTo ( y.DirEntry ) );
			}
		}

		internal class Header
		{
			public byte [] HeaderSignature { get; private set; } = new byte [] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };
			public byte [] CLSID { get; set; } = new byte [ 16 ];
			public ushort MinorVersion { get; private set; } = 0x003E;
			public ushort MajorVersion { get; private set; } = 0x0003;
			public ushort ByteOrder { get; private set; } = 0xFFFE;
			public ushort SectorShift { get; private set; } = 9;
			public ushort MiniSectorShift { get; private set; } = 6;
			public int DirectorySectorsNumber { get; set; }
			public int FATSectorsNumber { get; set; }
			public int FirstDirectorySectorID { get; set; } = Sector.ENDOFCHAIN;
			public uint MinSizeStandardStream { get; set; } = 4096;
			public int FirstMiniFATSectorID { get; set; } = unchecked(( int ) 0xFFFFFFFE);
			public uint MiniFATSectorsNumber { get; set; }
			public int FirstDIFATSectorID { get; set; } = Sector.ENDOFCHAIN;
			public uint DIFATSectorsNumber { get; set; }
			public int [] DIFAT { get; private set; } = new int [ 109 ];

			public Header () : this ( 3 ) { }
			public Header ( ushort version )
			{
				switch ( version )
				{
					case 3:
						MajorVersion = 3;
						SectorShift = 0x0009;
						break;

					case 4:
						MajorVersion = 4;
						SectorShift = 0x000C;
						break;

					default:
						throw new CFException ( "Invalid Compound File Format version" );
				}

				for ( int i = 0; i < 109; i++ )
					DIFAT [ i ] = Sector.FREESECT;
			}

			public void Read ( Stream stream )
			{
				using ( BinaryReader rw = new BinaryReader ( stream, Encoding.UTF8, true ) )
				{
					HeaderSignature = rw.ReadBytes ( 8 );
					CheckSignature ();
					CLSID = rw.ReadBytes ( 16 );
					MinorVersion = rw.ReadUInt16 ();
					MajorVersion = rw.ReadUInt16 ();
					CheckVersion ();
					ByteOrder = rw.ReadUInt16 ();
					SectorShift = rw.ReadUInt16 ();
					MiniSectorShift = rw.ReadUInt16 ();
					rw.ReadBytes ( 6 );
					DirectorySectorsNumber = rw.ReadInt32 ();
					FATSectorsNumber = rw.ReadInt32 ();
					FirstDirectorySectorID = rw.ReadInt32 ();
					rw.ReadUInt32 ();
					MinSizeStandardStream = rw.ReadUInt32 ();
					FirstMiniFATSectorID = rw.ReadInt32 ();
					MiniFATSectorsNumber = rw.ReadUInt32 ();
					FirstDIFATSectorID = rw.ReadInt32 ();
					DIFATSectorsNumber = rw.ReadUInt32 ();

					for ( int i = 0; i < 109; i++ )
						DIFAT [ i ] = rw.ReadInt32 ();
				}
			}

			private void CheckVersion ()
			{
				if ( MajorVersion != 3 && MajorVersion != 4 )
					throw new CFFileFormatException ( "Unsupported Binary File Format version: OpenMcdf only supports Compound Files with major version equal to 3 or 4 " );
			}

			private byte [] OLE_CFS_SIGNATURE = new byte [] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };

			private void CheckSignature ()
			{
				for ( int i = 0; i < HeaderSignature.Length; i++ )
					if ( HeaderSignature [ i ] != OLE_CFS_SIGNATURE [ i ] )
						throw new CFFileFormatException ( "Invalid OLE structured storage file" );
			}
		}


		internal class StreamView : Stream
		{
			private int sectorSize;
			private long position;

			private List<Sector> sectorChain;
			private Stream stream;
			private bool isFatStream = false;
			private List<Sector> freeSectors = new List<Sector> ();
			public IEnumerable<Sector> FreeSectors => freeSectors;

			public StreamView ( List<Sector> sectorChain, int sectorSize, Stream stream )
			{
				if ( sectorSize <= 0 )
					throw new CFException ( "Sector size must be greater than zero" );

				this.sectorChain = sectorChain ?? throw new CFException ( "Sector Chain cannot be null" );
				this.sectorSize = sectorSize;
				this.stream = stream;
			}

			public StreamView ( List<Sector> sectorChain, int sectorSize, long length, Queue<Sector> availableSectors, Stream stream, bool isFatStream = false )
				: this ( sectorChain, sectorSize, stream )
			{
				this.isFatStream = isFatStream;
				AdjustLength ( length, availableSectors );
			}

			public List<Sector> BaseSectorChain => sectorChain;

			public override bool CanRead => true;
			public override bool CanSeek => true;
			public override bool CanWrite => true;

			public override void Flush () { }

			private long length;
			public override long Length => length;
			public override long Position
			{
				get => position;
				set
				{
					if ( position > length - 1 )
						throw new ArgumentOutOfRangeException ( "value" );
					position = value;
				}
			}

			private byte [] buf = new byte [ 4 ];

			public int ReadInt32 ()
			{
				this.Read ( buf, 0, 4 );
				return ( ( ( buf [ 0 ] | ( buf [ 1 ] << 8 ) ) | ( buf [ 2 ] << 16 ) ) | ( buf [ 3 ] << 24 ) );
			}

			public override int Read ( byte [] buffer, int offset, int count )
			{
				int nRead = 0;
				int nToRead = 0;

				if ( sectorChain != null && sectorChain.Count > 0 )
				{
					// First sector
					int secIndex = ( int ) ( position / ( long ) sectorSize );

					// Bytes to read count is the min between request count
					// and sector border

					nToRead = Math.Min ( sectorChain [ 0 ].Size - ( ( int ) position % sectorSize ), count );

					if ( secIndex < sectorChain.Count )
					{
						Buffer.BlockCopy (
							sectorChain [ secIndex ].GetData (),
							( int ) ( position % sectorSize ),
							buffer, offset, nToRead );
					}

					nRead += nToRead;

					secIndex++;

					// Central sectors
					while ( nRead < ( count - sectorSize ) )
					{
						nToRead = sectorSize;
						Buffer.BlockCopy ( sectorChain [ secIndex ].GetData (), 0, buffer, offset + nRead, nToRead );
						nRead += nToRead;
						secIndex++;
					}

					// Last sector
					nToRead = count - nRead;

					if ( nToRead != 0 )
					{
						Buffer.BlockCopy ( sectorChain [ secIndex ].GetData (), 0, buffer, offset + nRead, nToRead );
						nRead += nToRead;
					}

					position += nRead;

					return nRead;

				}
				else
					return 0;

			}

			public override long Seek ( long offset, SeekOrigin origin )
			{
				switch ( origin )
				{
					case SeekOrigin.Begin: position = offset; break;
					case SeekOrigin.Current: position += offset; break;
					case SeekOrigin.End: position = Length - offset; break;
				}
				AdjustLength ( position );
				return position;
			}

			private void AdjustLength ( long value ) { AdjustLength ( value, null ); }

			private void AdjustLength ( long value, Queue<Sector> availableSectors )
			{
				this.length = value;
				long delta = value - ( ( long ) this.sectorChain.Count * ( long ) sectorSize );

				if ( delta > 0 )
				{
					int nSec = ( int ) Math.Ceiling ( ( ( double ) delta / sectorSize ) );
					while ( nSec > 0 )
					{
						Sector t = null;

						if ( availableSectors == null || availableSectors.Count == 0 )
						{
							t = new Sector ( sectorSize, stream );
							if ( sectorSize == Sector.MINISECTOR_SIZE )
								t.Type = SectorType.Mini;
						}
						else
							t = availableSectors.Dequeue ();

						if ( isFatStream )
							t.InitFATData ();
						sectorChain.Add ( t );
						nSec--;
					}
				}
			}

			public override void SetLength ( long value )
			{
				AdjustLength ( value );
			}

			public override void Write ( byte [] buffer, int offset, int count ) => throw new NotImplementedException ();
		}

		[Flags]
		internal enum CFSConfiguration
		{
			Default = 1,
			SectorRecycle = 2,
			EraseFreeSectors = 4,
			NoValidationException = 8,
			LeaveOpen = 16,
		}

		internal enum CFSVersion : int
		{
			Ver_3 = 3,
			Ver_4 = 4
		}

		internal enum CFSUpdateMode
		{
			ReadOnly,
			Update
		}

		internal class CompoundFile : IDisposable
		{
			private CFSConfiguration configuration
				= CFSConfiguration.Default;

			public CFSConfiguration Configuration
			{
				get
				{
					return configuration;
				}
			}
			internal int GetSectorSize ()
			{
				return 2 << ( header.SectorShift - 1 );
			}

			private const int HEADER_DIFAT_ENTRIES_COUNT = 109;
			private readonly int DIFAT_SECTOR_FAT_ENTRIES_COUNT = 127;
			private readonly int FAT_SECTOR_ENTRIES_COUNT = 128;
			private const int SIZE_OF_SID = 4;
			private const int FLUSHING_QUEUE_SIZE = 6000;
			private const int FLUSHING_BUFFER_MAX_SIZE = 1024 * 1024 * 16;

			private bool sectorRecycle = false;
			private bool eraseFreeSectors = false;

			private SectorCollection sectors = new SectorCollection ();

			private Header header;

			internal Stream sourceStream = null;

			public CompoundFile ()
			{
				this.header = new Header ();
				this.sectorRecycle = false;

				this.sectors.OnVer3SizeLimitReached += new Ver3SizeLimitReached ( OnSizeLimitReached );

				DIFAT_SECTOR_FAT_ENTRIES_COUNT = ( GetSectorSize () / 4 ) - 1;
				FAT_SECTOR_ENTRIES_COUNT = ( GetSectorSize () / 4 );

				IDirectoryEntry de = DirectoryEntry.New ( "Root Entry", StgType.StgRoot, directoryEntries );
				rootStorage = new CFStorage ( this, de );
				rootStorage.DirEntry.StgType = StgType.StgRoot;
				rootStorage.DirEntry.StgColor = StgColor.Black;
			}

			void OnSizeLimitReached ()
			{
				Sector rangeLockSector = new Sector ( GetSectorSize (), sourceStream );
				sectors.Add ( rangeLockSector );

				rangeLockSector.Type = SectorType.RangeLockSector;

				_transactionLockAdded = true;
				_lockSectorId = rangeLockSector.Id;
			}

			public CompoundFile ( CFSVersion cfsVersion, CFSConfiguration configFlags )
			{
				this.configuration = configFlags;

				bool sectorRecycle = configFlags.HasFlag ( CFSConfiguration.SectorRecycle );
				bool eraseFreeSectors = configFlags.HasFlag ( CFSConfiguration.EraseFreeSectors );

				this.header = new Header ( ( ushort ) cfsVersion );
				this.sectorRecycle = sectorRecycle;


				DIFAT_SECTOR_FAT_ENTRIES_COUNT = ( GetSectorSize () / 4 ) - 1;
				FAT_SECTOR_ENTRIES_COUNT = ( GetSectorSize () / 4 );

				//Root -- 
				IDirectoryEntry rootDir = DirectoryEntry.New ( "Root Entry", StgType.StgRoot, directoryEntries );
				rootDir.StgColor = StgColor.Black;
				//this.InsertNewDirectoryEntry(rootDir);

				rootStorage = new CFStorage ( this, rootDir );
			}

			public CompoundFile ( String fileName )
			{
				this.sectorRecycle = false;
				this.updateMode = CFSUpdateMode.ReadOnly;
				this.eraseFreeSectors = false;

				LoadFile ( fileName );

				DIFAT_SECTOR_FAT_ENTRIES_COUNT = ( GetSectorSize () / 4 ) - 1;
				FAT_SECTOR_ENTRIES_COUNT = ( GetSectorSize () / 4 );
			}

			public CompoundFile ( String fileName, CFSUpdateMode updateMode, CFSConfiguration configParameters )
			{
				this.validationExceptionEnabled = !configParameters.HasFlag ( CFSConfiguration.NoValidationException );
				this.sectorRecycle = configParameters.HasFlag ( CFSConfiguration.SectorRecycle );
				this.updateMode = updateMode;
				this.eraseFreeSectors = configParameters.HasFlag ( CFSConfiguration.EraseFreeSectors );

				LoadFile ( fileName );

				DIFAT_SECTOR_FAT_ENTRIES_COUNT = ( GetSectorSize () / 4 ) - 1;
				FAT_SECTOR_ENTRIES_COUNT = ( GetSectorSize () / 4 );
			}

			private bool validationExceptionEnabled = true;

			public bool ValidationExceptionEnabled
			{
				get { return validationExceptionEnabled; }
			}

			public CompoundFile ( Stream stream, CFSUpdateMode updateMode, CFSConfiguration configParameters )
			{
				this.validationExceptionEnabled = !configParameters.HasFlag ( CFSConfiguration.NoValidationException );
				this.sectorRecycle = configParameters.HasFlag ( CFSConfiguration.SectorRecycle );
				this.eraseFreeSectors = configParameters.HasFlag ( CFSConfiguration.EraseFreeSectors );
				this.closeStream = !configParameters.HasFlag ( CFSConfiguration.LeaveOpen );

				this.updateMode = updateMode;
				LoadStream ( stream );

				DIFAT_SECTOR_FAT_ENTRIES_COUNT = ( GetSectorSize () / 4 ) - 1;
				FAT_SECTOR_ENTRIES_COUNT = ( GetSectorSize () / 4 );
			}

			public CompoundFile ( Stream stream )
			{
				LoadStream ( stream );

				DIFAT_SECTOR_FAT_ENTRIES_COUNT = ( GetSectorSize () / 4 ) - 1;
				FAT_SECTOR_ENTRIES_COUNT = ( GetSectorSize () / 4 );
			}

			private CFSUpdateMode updateMode = CFSUpdateMode.ReadOnly;
			private string fileName = string.Empty;

			private void Load ( Stream stream )
			{
				try
				{
					this.header = new Header ();
					this.directoryEntries = new List<IDirectoryEntry> ();

					this.sourceStream = stream;

					header.Read ( stream );

					int n_sector = Ceiling ( ( ( double ) ( stream.Length - GetSectorSize () ) / ( double ) GetSectorSize () ) );

					if ( stream.Length > 0x7FFFFF0 )
						this._transactionLockAllocated = true;


					sectors = new SectorCollection ();
					//sectors = new ArrayList();
					for ( int i = 0; i < n_sector; i++ )
					{
						sectors.Add ( null );
					}

					LoadDirectories ();

					this.rootStorage
						= new CFStorage ( this, directoryEntries [ 0 ] );
				}
				catch ( Exception )
				{
					if ( stream != null && closeStream )
						stream.Dispose ();

					throw;
				}
			}

			private void LoadFile ( String fileName )
			{
				this.fileName = fileName;
				FileStream fs = null;

				try
				{
					if ( this.updateMode == CFSUpdateMode.ReadOnly )
					{
						fs = new FileStream ( fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite );

					}
					else
					{
						fs = new FileStream ( fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read );
					}

					Load ( fs );

				}
				catch
				{
					if ( fs != null )
						fs.Dispose ();

					throw;
				}
			}

			private void LoadStream ( Stream stream )
			{
				if ( stream == null )
					throw new CFException ( "Stream parameter cannot be null" );

				if ( !stream.CanSeek )
					throw new CFException ( "Cannot load a non-seekable Stream" );


				stream.Seek ( 0, SeekOrigin.Begin );

				Load ( stream );
			}

			public bool HasSourceStream
			{
				get { return sourceStream != null; }
			}

			private void PersistMiniStreamToStream ( List<Sector> miniSectorChain )
			{
				List<Sector> miniStream
					= GetSectorChain ( RootEntry.StartSetc, SectorType.Normal );

				StreamView miniStreamView
					= new StreamView (
						miniStream,
						GetSectorSize (),
						this.rootStorage.Size,
						null,
						sourceStream );

				for ( int i = 0; i < miniSectorChain.Count; i++ )
				{
					Sector s = miniSectorChain [ i ];

					if ( s.Id == -1 )
						throw new CFException ( "Invalid minisector index" );

					// Ministream sectors already allocated
					miniStreamView.Seek ( Sector.MINISECTOR_SIZE * s.Id, SeekOrigin.Begin );
					miniStreamView.Write ( s.GetData (), 0, Sector.MINISECTOR_SIZE );
				}
			}

			private void AllocateMiniSectorChain ( List<Sector> sectorChain )
			{
				List<Sector> miniFAT
					= GetSectorChain ( header.FirstMiniFATSectorID, SectorType.Normal );

				List<Sector> miniStream
					= GetSectorChain ( RootEntry.StartSetc, SectorType.Normal );

				StreamView miniFATView
					= new StreamView (
						miniFAT,
						GetSectorSize (),
						header.MiniFATSectorsNumber * Sector.MINISECTOR_SIZE,
						null,
						this.sourceStream,
						true
						);

				StreamView miniStreamView
					= new StreamView (
						miniStream,
						GetSectorSize (),
						this.rootStorage.Size,
						null,
						sourceStream );

				for ( int i = 0; i < sectorChain.Count; i++ )
				{
					Sector s = sectorChain [ i ];

					if ( s.Id == -1 )
					{
						miniStreamView.Seek ( this.rootStorage.Size + Sector.MINISECTOR_SIZE, SeekOrigin.Begin );
						s.Id = ( int ) ( miniStreamView.Position - Sector.MINISECTOR_SIZE ) / Sector.MINISECTOR_SIZE;

						this.rootStorage.DirEntry.Size = miniStreamView.Length;
					}
				}

				// Update miniFAT
				for ( int i = 0; i < sectorChain.Count - 1; i++ )
				{
					Int32 currentId = sectorChain [ i ].Id;
					Int32 nextId = sectorChain [ i + 1 ].Id;

					miniFATView.Seek ( currentId * 4, SeekOrigin.Begin );
					miniFATView.Write ( BitConverter.GetBytes ( nextId ), 0, 4 );
				}

				// Write End of Chain in MiniFAT
				miniFATView.Seek ( sectorChain [ sectorChain.Count - 1 ].Id * SIZE_OF_SID, SeekOrigin.Begin );
				miniFATView.Write ( BitConverter.GetBytes ( Sector.ENDOFCHAIN ), 0, 4 );

				// Update sector chains
				AllocateSectorChain ( miniStreamView.BaseSectorChain );
				AllocateSectorChain ( miniFATView.BaseSectorChain );

				//Update HEADER and root storage when ministream changes
				if ( miniFAT.Count > 0 )
				{
					this.rootStorage.DirEntry.StartSetc = miniStream [ 0 ].Id;
					header.MiniFATSectorsNumber = ( uint ) miniFAT.Count;
					header.FirstMiniFATSectorID = miniFAT [ 0 ].Id;
				}
			}

			internal void FreeData ( CFStream stream )
			{
				if ( stream.Size == 0 )
					return;

				List<Sector> sectorChain = null;

				if ( stream.Size < header.MinSizeStandardStream )
				{
					sectorChain = GetSectorChain ( stream.DirEntry.StartSetc, SectorType.Mini );
					FreeMiniChain ( sectorChain, this.eraseFreeSectors );
				}
				else
				{
					sectorChain = GetSectorChain ( stream.DirEntry.StartSetc, SectorType.Normal );
					FreeChain ( sectorChain, this.eraseFreeSectors );
				}

				stream.DirEntry.StartSetc = Sector.ENDOFCHAIN;
				stream.DirEntry.Size = 0;
			}

			private void FreeChain ( List<Sector> sectorChain, bool zeroSector )
			{
				FreeChain ( sectorChain, 0, zeroSector );
			}

			private void FreeChain ( List<Sector> sectorChain, int nth_sector_to_remove, bool zeroSector )
			{
				// Dummy zero buffer
				byte [] ZEROED_SECTOR = new byte [ GetSectorSize () ];

				List<Sector> FAT
					= GetSectorChain ( -1, SectorType.FAT );

				StreamView FATView
					= new StreamView ( FAT, GetSectorSize (), FAT.Count * GetSectorSize (), null, sourceStream );

				// Zeroes out sector data (if required)-------------
				if ( zeroSector )
				{
					for ( int i = nth_sector_to_remove; i < sectorChain.Count; i++ )
					{
						Sector s = sectorChain [ i ];
						s.ZeroData ();
					}
				}

				// Update FAT marking unallocated sectors ----------
				for ( int i = nth_sector_to_remove; i < sectorChain.Count; i++ )
				{
					Int32 currentId = sectorChain [ i ].Id;

					FATView.Seek ( currentId * 4, SeekOrigin.Begin );
					FATView.Write ( BitConverter.GetBytes ( Sector.FREESECT ), 0, 4 );
				}

				// Write new end of chain if partial free ----------
				if ( nth_sector_to_remove > 0 && sectorChain.Count > 0 )
				{
					FATView.Seek ( sectorChain [ nth_sector_to_remove - 1 ].Id * 4, SeekOrigin.Begin );
					FATView.Write ( BitConverter.GetBytes ( Sector.ENDOFCHAIN ), 0, 4 );
				}
			}

			private void FreeMiniChain ( List<Sector> sectorChain, bool zeroSector )
			{
				FreeMiniChain ( sectorChain, 0, zeroSector );
			}

			private void FreeMiniChain ( List<Sector> sectorChain, int nth_sector_to_remove, bool zeroSector )
			{
				byte [] ZEROED_MINI_SECTOR = new byte [ Sector.MINISECTOR_SIZE ];

				List<Sector> miniFAT
					= GetSectorChain ( header.FirstMiniFATSectorID, SectorType.Normal );

				List<Sector> miniStream
					= GetSectorChain ( RootEntry.StartSetc, SectorType.Normal );

				StreamView miniFATView
					= new StreamView ( miniFAT, GetSectorSize (), header.MiniFATSectorsNumber * Sector.MINISECTOR_SIZE, null, sourceStream );

				StreamView miniStreamView
					= new StreamView ( miniStream, GetSectorSize (), this.rootStorage.Size, null, sourceStream );

				// Set updated/new sectors within the ministream ----------
				if ( zeroSector )
				{
					for ( int i = nth_sector_to_remove; i < sectorChain.Count; i++ )
					{
						Sector s = sectorChain [ i ];

						if ( s.Id != -1 )
						{
							// Overwrite
							miniStreamView.Seek ( Sector.MINISECTOR_SIZE * s.Id, SeekOrigin.Begin );
							miniStreamView.Write ( ZEROED_MINI_SECTOR, 0, Sector.MINISECTOR_SIZE );
						}
					}
				}

				// Update miniFAT                ---------------------------------------
				for ( int i = nth_sector_to_remove; i < sectorChain.Count; i++ )
				{
					Int32 currentId = sectorChain [ i ].Id;

					miniFATView.Seek ( currentId * 4, SeekOrigin.Begin );
					miniFATView.Write ( BitConverter.GetBytes ( Sector.FREESECT ), 0, 4 );
				}

				if ( nth_sector_to_remove > 0 && sectorChain.Count > 0 )
				{
					miniFATView.Seek ( sectorChain [ nth_sector_to_remove - 1 ].Id * 4, SeekOrigin.Begin );
					miniFATView.Write ( BitConverter.GetBytes ( Sector.ENDOFCHAIN ), 0, 4 );
				}

				// Update sector chains           ---------------------------------------
				AllocateSectorChain ( miniStreamView.BaseSectorChain );
				AllocateSectorChain ( miniFATView.BaseSectorChain );

				//Update HEADER and root storage when ministream changes
				if ( miniFAT.Count > 0 )
				{
					this.rootStorage.DirEntry.StartSetc = miniStream [ 0 ].Id;
					header.MiniFATSectorsNumber = ( uint ) miniFAT.Count;
					header.FirstMiniFATSectorID = miniFAT [ 0 ].Id;
				}
			}

			private void SetSectorChain ( List<Sector> sectorChain )
			{
				if ( sectorChain == null || sectorChain.Count == 0 )
					return;

				SectorType _st = sectorChain [ 0 ].Type;

				if ( _st == SectorType.Normal )
				{
					AllocateSectorChain ( sectorChain );
				}
				else if ( _st == SectorType.Mini )
				{
					AllocateMiniSectorChain ( sectorChain );
				}
			}

			private void AllocateSectorChain ( List<Sector> sectorChain )
			{

				foreach ( Sector s in sectorChain )
				{
					if ( s.Id == -1 )
					{
						sectors.Add ( s );
						s.Id = sectors.Count - 1;

					}
				}

				AllocateFATSectorChain ( sectorChain );
			}

			internal bool _transactionLockAdded = false;
			internal int _lockSectorId = -1;
			internal bool _transactionLockAllocated = false;

			private void CheckForLockSector ()
			{
				//If transaction lock has been added and not yet allocated in the FAT...
				if ( _transactionLockAdded && !_transactionLockAllocated )
				{
					StreamView fatStream = new StreamView ( GetFatSectorChain (), GetSectorSize (), sourceStream );

					fatStream.Seek ( _lockSectorId * 4, SeekOrigin.Begin );
					fatStream.Write ( BitConverter.GetBytes ( Sector.ENDOFCHAIN ), 0, 4 );

					_transactionLockAllocated = true;
				}

			}
			private void AllocateFATSectorChain ( List<Sector> sectorChain )
			{
				List<Sector> fatSectors = GetSectorChain ( -1, SectorType.FAT );

				StreamView fatStream =
					new StreamView (
						fatSectors,
						GetSectorSize (),
						header.FATSectorsNumber * GetSectorSize (),
						null,
						sourceStream,
						true
						);

				// Write FAT chain values --

				for ( int i = 0; i < sectorChain.Count - 1; i++ )
				{

					Sector sN = sectorChain [ i + 1 ];
					Sector sC = sectorChain [ i ];

					fatStream.Seek ( sC.Id * 4, SeekOrigin.Begin );
					fatStream.Write ( BitConverter.GetBytes ( sN.Id ), 0, 4 );
				}

				fatStream.Seek ( sectorChain [ sectorChain.Count - 1 ].Id * 4, SeekOrigin.Begin );
				fatStream.Write ( BitConverter.GetBytes ( Sector.ENDOFCHAIN ), 0, 4 );

				// Merge chain to CFS
				AllocateDIFATSectorChain ( fatStream.BaseSectorChain );
			}

			private void AllocateDIFATSectorChain ( List<Sector> FATsectorChain )
			{
				// Get initial sector's count
				header.FATSectorsNumber = FATsectorChain.Count;

				// Allocate Sectors
				foreach ( Sector s in FATsectorChain )
				{
					if ( s.Id == -1 )
					{
						sectors.Add ( s );
						s.Id = sectors.Count - 1;
						s.Type = SectorType.FAT;
					}
				}

				// Sector count...
				int nCurrentSectors = sectors.Count;

				// Temp DIFAT count
				int nDIFATSectors = ( int ) header.DIFATSectorsNumber;

				if ( FATsectorChain.Count > HEADER_DIFAT_ENTRIES_COUNT )
				{
					nDIFATSectors = Ceiling ( ( double ) ( FATsectorChain.Count - HEADER_DIFAT_ENTRIES_COUNT ) / DIFAT_SECTOR_FAT_ENTRIES_COUNT );
					nDIFATSectors = LowSaturation ( nDIFATSectors - ( int ) header.DIFATSectorsNumber ); //required DIFAT
				}

				// ...sum with new required DIFAT sectors count
				nCurrentSectors += nDIFATSectors;

				// ReCheck FAT bias
				while ( header.FATSectorsNumber * FAT_SECTOR_ENTRIES_COUNT < nCurrentSectors )
				{
					Sector extraFATSector = new Sector ( GetSectorSize (), sourceStream );
					sectors.Add ( extraFATSector );

					extraFATSector.Id = sectors.Count - 1;
					extraFATSector.Type = SectorType.FAT;

					FATsectorChain.Add ( extraFATSector );

					header.FATSectorsNumber++;
					nCurrentSectors++;

					//... so, adding a FAT sector may induce DIFAT sectors to increase by one
					// and consequently this may induce ANOTHER FAT sector (TO-THINK: May this condition occure ?)
					if ( nDIFATSectors * DIFAT_SECTOR_FAT_ENTRIES_COUNT <
						( header.FATSectorsNumber > HEADER_DIFAT_ENTRIES_COUNT ?
						header.FATSectorsNumber - HEADER_DIFAT_ENTRIES_COUNT :
						0 ) )
					{
						nDIFATSectors++;
						nCurrentSectors++;
					}
				}


				List<Sector> difatSectors =
							GetSectorChain ( -1, SectorType.DIFAT );

				StreamView difatStream
					= new StreamView ( difatSectors, GetSectorSize (), sourceStream );

				// Write DIFAT Sectors (if required)
				// Save room for the following chaining
				for ( int i = 0; i < FATsectorChain.Count; i++ )
				{
					if ( i < HEADER_DIFAT_ENTRIES_COUNT )
					{
						header.DIFAT [ i ] = FATsectorChain [ i ].Id;
					}
					else
					{
						// room for DIFAT chaining at the end of any DIFAT sector (4 bytes)
						if ( i != HEADER_DIFAT_ENTRIES_COUNT && ( i - HEADER_DIFAT_ENTRIES_COUNT ) % DIFAT_SECTOR_FAT_ENTRIES_COUNT == 0 )
						{
							byte [] temp = new byte [ sizeof ( int ) ];
							difatStream.Write ( temp, 0, sizeof ( int ) );
						}

						difatStream.Write ( BitConverter.GetBytes ( FATsectorChain [ i ].Id ), 0, sizeof ( int ) );

					}
				}

				// Allocate room for DIFAT sectors
				for ( int i = 0; i < difatStream.BaseSectorChain.Count; i++ )
				{
					if ( difatStream.BaseSectorChain [ i ].Id == -1 )
					{
						sectors.Add ( difatStream.BaseSectorChain [ i ] );
						difatStream.BaseSectorChain [ i ].Id = sectors.Count - 1;
						difatStream.BaseSectorChain [ i ].Type = SectorType.DIFAT;
					}
				}

				header.DIFATSectorsNumber = ( uint ) nDIFATSectors;


				// Chain first sector
				if ( difatStream.BaseSectorChain != null && difatStream.BaseSectorChain.Count > 0 )
				{
					header.FirstDIFATSectorID = difatStream.BaseSectorChain [ 0 ].Id;

					// Update header information
					header.DIFATSectorsNumber = ( uint ) difatStream.BaseSectorChain.Count;

					// Write chaining information at the end of DIFAT Sectors
					for ( int i = 0; i < difatStream.BaseSectorChain.Count - 1; i++ )
					{
						Buffer.BlockCopy (
							BitConverter.GetBytes ( difatStream.BaseSectorChain [ i + 1 ].Id ),
							0,
							difatStream.BaseSectorChain [ i ].GetData (),
							GetSectorSize () - sizeof ( int ),
							4 );
					}

					Buffer.BlockCopy (
						BitConverter.GetBytes ( Sector.ENDOFCHAIN ),
						0,
						difatStream.BaseSectorChain [ difatStream.BaseSectorChain.Count - 1 ].GetData (),
						GetSectorSize () - sizeof ( int ),
						sizeof ( int )
						);
				}
				else
					header.FirstDIFATSectorID = Sector.ENDOFCHAIN;

				// Mark DIFAT Sectors in FAT
				StreamView fatSv = new StreamView ( FATsectorChain, GetSectorSize (), header.FATSectorsNumber * GetSectorSize (), null, sourceStream );

				for ( int i = 0; i < header.DIFATSectorsNumber; i++ )
				{
					fatSv.Seek ( difatStream.BaseSectorChain [ i ].Id * 4, SeekOrigin.Begin );
					fatSv.Write ( BitConverter.GetBytes ( Sector.DIFSECT ), 0, 4 );
				}

				for ( int i = 0; i < header.FATSectorsNumber; i++ )
				{
					fatSv.Seek ( fatSv.BaseSectorChain [ i ].Id * 4, SeekOrigin.Begin );
					fatSv.Write ( BitConverter.GetBytes ( Sector.FATSECT ), 0, 4 );
				}

				header.FATSectorsNumber = fatSv.BaseSectorChain.Count;
			}

			private List<Sector> GetDifatSectorChain ()
			{
				int validationCount = 0;

				List<Sector> result = new List<Sector> ();

				int nextSecID
				   = Sector.ENDOFCHAIN;

				if ( header.DIFATSectorsNumber != 0 )
				{
					validationCount = ( int ) header.DIFATSectorsNumber;

					Sector s = sectors [ header.FirstDIFATSectorID ] as Sector;

					if ( s == null ) //Lazy loading
					{
						sectors [ header.FirstDIFATSectorID ] = s = new Sector ( GetSectorSize (), sourceStream )
						{
							Type = SectorType.DIFAT,
							Id = header.FirstDIFATSectorID
						};
					}

					result.Add ( s );

					while ( true && validationCount >= 0 )
					{
						nextSecID = BitConverter.ToInt32 ( s.GetData (), GetSectorSize () - 4 );

						if ( nextSecID == Sector.FREESECT || nextSecID == Sector.ENDOFCHAIN ) break;

						validationCount--;

						if ( validationCount < 0 )
						{
							Dispose ( true );
							throw new CFCorruptedFileException ( "DIFAT sectors count mismatched. Corrupted compound file" );
						}

						s = sectors [ nextSecID ] as Sector;

						if ( s == null )
							sectors [ nextSecID ] = s = new Sector ( GetSectorSize (), sourceStream ) { Id = nextSecID };

						result.Add ( s );
					}
				}

				return result;
			}

			private List<Sector> GetFatSectorChain ()
			{
				int N_HEADER_FAT_ENTRY = 109; //Number of FAT sectors id in the header

				List<Sector> result
				   = new List<Sector> ();

				int nextSecID
				   = Sector.ENDOFCHAIN;

				List<Sector> difatSectors = GetDifatSectorChain ();

				int idx = 0;

				// Read FAT entries from the header Fat entry array (max 109 entries)
				while ( idx < header.FATSectorsNumber && idx < N_HEADER_FAT_ENTRY )
				{
					nextSecID = header.DIFAT [ idx ];
					Sector s = sectors [ nextSecID ] as Sector;

					if ( s == null )
					{
						sectors [ nextSecID ] = s = new Sector ( GetSectorSize (), sourceStream )
						{
							Id = nextSecID,
							Type = SectorType.FAT
						};
					}

					result.Add ( s );

					idx++;
				}

				//Is there any DIFAT sector containing other FAT entries ?
				if ( difatSectors.Count > 0 )
				{
					StreamView difatStream
						= new StreamView ( difatSectors, GetSectorSize (),
							header.FATSectorsNumber > N_HEADER_FAT_ENTRY ? ( header.FATSectorsNumber - N_HEADER_FAT_ENTRY ) * 4 : 0,
							null, sourceStream );

					byte [] nextDIFATSectorBuffer = new byte [ 4 ];

					difatStream.Read ( nextDIFATSectorBuffer, 0, 4 );
					nextSecID = BitConverter.ToInt32 ( nextDIFATSectorBuffer, 0 );

					int i = 0;
					int nFat = N_HEADER_FAT_ENTRY;

					while ( nFat < header.FATSectorsNumber )
					{
						if ( difatStream.Position == ( ( GetSectorSize () - 4 ) + i * GetSectorSize () ) )
						{
							difatStream.Seek ( 4, SeekOrigin.Current );
							i++;
							continue;
						}

						Sector s = sectors [ nextSecID ] as Sector;

						if ( s == null )
						{
							sectors [ nextSecID ] = s = new Sector ( GetSectorSize (), sourceStream )
							{
								Type = SectorType.FAT,
								Id = nextSecID
							};//UUU
						}

						result.Add ( s );

						difatStream.Read ( nextDIFATSectorBuffer, 0, 4 );
						nextSecID = BitConverter.ToInt32 ( nextDIFATSectorBuffer, 0 );
						nFat++;
					}
				}

				return result;

			}

			private List<Sector> GetNormalSectorChain ( int secID )
			{
				List<Sector> result = new List<Sector> ();

				int nextSecID = secID;

				List<Sector> fatSectors = GetFatSectorChain ();

				StreamView fatStream
					= new StreamView ( fatSectors, GetSectorSize (), fatSectors.Count * GetSectorSize (), null, sourceStream );

				while ( true )
				{
					if ( nextSecID == Sector.ENDOFCHAIN ) break;

					if ( nextSecID < 0 )
						throw new CFCorruptedFileException ( String.Format ( "Next Sector ID reference is below zero. NextID : {0}", nextSecID ) );

					if ( nextSecID >= sectors.Count )
						throw new CFCorruptedFileException ( String.Format ( "Next Sector ID reference an out of range sector. NextID : {0} while sector count {1}", nextSecID, sectors.Count ) );

					Sector s = sectors [ nextSecID ] as Sector;
					if ( s == null )
					{
						sectors [ nextSecID ] = s = new Sector ( GetSectorSize (), sourceStream )
						{
							Id = nextSecID,
							Type = SectorType.Normal
						};
					}

					result.Add ( s );

					fatStream.Seek ( nextSecID * 4, SeekOrigin.Begin );
					int next = fatStream.ReadInt32 ();

					if ( next != nextSecID )
						nextSecID = next;
					else
						throw new CFCorruptedFileException ( "Cyclic sector chain found. File is corrupted" );
				}


				return result;
			}

			private List<Sector> GetMiniSectorChain ( int secID )
			{
				List<Sector> result
					  = new List<Sector> ();

				if ( secID != Sector.ENDOFCHAIN )
				{
					int nextSecID = secID;

					List<Sector> miniFAT = GetNormalSectorChain ( header.FirstMiniFATSectorID );
					List<Sector> miniStream = GetNormalSectorChain ( RootEntry.StartSetc );

					StreamView miniFATView
						= new StreamView ( miniFAT, GetSectorSize (), header.MiniFATSectorsNumber * Sector.MINISECTOR_SIZE, null, sourceStream );

					StreamView miniStreamView =
						new StreamView ( miniStream, GetSectorSize (), rootStorage.Size, null, sourceStream );

					BinaryReader miniFATReader = new BinaryReader ( miniFATView );

					nextSecID = secID;

					while ( true )
					{
						if ( nextSecID == Sector.ENDOFCHAIN )
							break;

						Sector ms = new Sector ( Sector.MINISECTOR_SIZE, sourceStream );
						byte [] temp = new byte [ Sector.MINISECTOR_SIZE ];

						ms.Id = nextSecID;
						ms.Type = SectorType.Mini;

						miniStreamView.Seek ( nextSecID * Sector.MINISECTOR_SIZE, SeekOrigin.Begin );
						miniStreamView.Read ( ms.GetData (), 0, Sector.MINISECTOR_SIZE );

						result.Add ( ms );

						miniFATView.Seek ( nextSecID * 4, SeekOrigin.Begin );
						nextSecID = miniFATReader.ReadInt32 ();
					}
				}
				return result;
			}

			internal List<Sector> GetSectorChain ( int secID, SectorType chainType )
			{

				switch ( chainType )
				{
					case SectorType.DIFAT:
						return GetDifatSectorChain ();

					case SectorType.FAT:
						return GetFatSectorChain ();

					case SectorType.Normal:
						return GetNormalSectorChain ( secID );

					case SectorType.Mini:
						return GetMiniSectorChain ( secID );

					default:
						throw new CFException ( "Unsupproted chain type" );
				}
			}

			private CFStorage rootStorage;

			public CFStorage RootStorage
			{
				get
				{
					return rootStorage as CFStorage;
				}
			}

			public CFSVersion Version
			{
				get
				{
					return ( CFSVersion ) this.header.MajorVersion;
				}
			}

			internal RBTree CreateNewTree ()
			{
				RBTree bst = new RBTree ();
				return bst;
			}

			internal RBTree GetChildrenTree ( int sid )
			{
				RBTree bst = new RBTree ();

				// Load children from their original tree.
				DoLoadChildren ( bst, directoryEntries [ sid ] );
				//bst = DoLoadChildrenTrusted(directoryEntries[sid]);

				return bst;
			}

			private RBTree DoLoadChildrenTrusted ( IDirectoryEntry de )
			{
				RBTree bst = null;

				if ( de.Child != DirectoryEntry.NOSTREAM )
				{
					bst = new RBTree ( directoryEntries [ de.Child ] );
				}

				return bst;
			}


			private void DoLoadChildren ( RBTree bst, IDirectoryEntry de )
			{

				if ( de.Child != DirectoryEntry.NOSTREAM )
				{
					if ( directoryEntries [ de.Child ].StgType == StgType.StgInvalid ) return;

					LoadSiblings ( bst, directoryEntries [ de.Child ] );
					NullifyChildNodes ( directoryEntries [ de.Child ] );
					bst.Insert ( directoryEntries [ de.Child ] );
				}
			}

			private void NullifyChildNodes ( IDirectoryEntry de )
			{
				de.Parent = null;
				de.Left = null;
				de.Right = null;
			}

			private List<int> levelSIDs = new List<int> ();

			private void LoadSiblings ( RBTree bst, IDirectoryEntry de )
			{
				levelSIDs.Clear ();

				if ( de.LeftSibling != DirectoryEntry.NOSTREAM )
				{


					// If there're more left siblings load them...
					DoLoadSiblings ( bst, directoryEntries [ de.LeftSibling ] );
					//NullifyChildNodes(directoryEntries[de.LeftSibling]);
				}

				if ( de.RightSibling != DirectoryEntry.NOSTREAM )
				{
					levelSIDs.Add ( de.RightSibling );

					// If there're more right siblings load them...
					DoLoadSiblings ( bst, directoryEntries [ de.RightSibling ] );
					//NullifyChildNodes(directoryEntries[de.RightSibling]);
				}
			}

			private void DoLoadSiblings ( RBTree bst, IDirectoryEntry de )
			{
				if ( ValidateSibling ( de.LeftSibling ) )
				{
					levelSIDs.Add ( de.LeftSibling );

					// If there're more left siblings load them...
					DoLoadSiblings ( bst, directoryEntries [ de.LeftSibling ] );
				}

				if ( ValidateSibling ( de.RightSibling ) )
				{
					levelSIDs.Add ( de.RightSibling );

					// If there're more right siblings load them...
					DoLoadSiblings ( bst, directoryEntries [ de.RightSibling ] );
				}

				NullifyChildNodes ( de );
				bst.Insert ( de );
			}

			private bool ValidateSibling ( int sid )
			{
				if ( sid != DirectoryEntry.NOSTREAM )
				{
					// if this siblings id does not overflow current list
					if ( sid >= directoryEntries.Count )
					{
						if ( this.validationExceptionEnabled )
						{
							//this.Close();
							throw new CFCorruptedFileException ( "A Directory Entry references the non-existent sid number " + sid.ToString () );
						}
						else
							return false;
					}

					//if this sibling is valid...
					if ( directoryEntries [ sid ].StgType == StgType.StgInvalid )
					{
						if ( this.validationExceptionEnabled )
						{
							//this.Close();
							throw new CFCorruptedFileException ( "A Directory Entry has a valid reference to an Invalid Storage Type directory [" + sid + "]" );
						}
						else
							return false;
					}

					if ( !Enum.IsDefined ( typeof ( StgType ), directoryEntries [ sid ].StgType ) )
					{

						if ( this.validationExceptionEnabled )
						{
							//this.Close();
							throw new CFCorruptedFileException ( "A Directory Entry has an invalid Storage Type" );
						}
						else
							return false;
					}

					if ( levelSIDs.Contains ( sid ) )
						throw new CFCorruptedFileException ( "Cyclic reference of directory item" );

					return true; //No fault condition encountered for sid being validated
				}

				return false;
			}

			private void LoadDirectories ()
			{
				List<Sector> directoryChain
					= GetSectorChain ( header.FirstDirectorySectorID, SectorType.Normal );

				if ( header.FirstDirectorySectorID == Sector.ENDOFCHAIN )
					header.FirstDirectorySectorID = directoryChain [ 0 ].Id;

				StreamView dirReader
					= new StreamView ( directoryChain, GetSectorSize (), directoryChain.Count * GetSectorSize (), null, sourceStream );


				while ( dirReader.Position < directoryChain.Count * GetSectorSize () )
				{
					IDirectoryEntry de
					= DirectoryEntry.New ( String.Empty, StgType.StgInvalid, directoryEntries );

					//We are not inserting dirs. Do not use 'InsertNewDirectoryEntry'
					de.Read ( dirReader, this.Version );

				}
			}

			internal Queue<Sector> FindFreeSectors ( SectorType sType )
			{
				Queue<Sector> freeList = new Queue<Sector> ();

				if ( sType == SectorType.Normal )
				{
					List<Sector> FatChain = GetSectorChain ( -1, SectorType.FAT );
					StreamView fatStream = new StreamView ( FatChain, GetSectorSize (), header.FATSectorsNumber * GetSectorSize (), null, sourceStream );

					int idx = 0;
					while ( idx < sectors.Count )
					{
						int id = fatStream.ReadInt32 ();

						if ( id == Sector.FREESECT )
						{
							if ( sectors [ idx ] == null )
							{
								sectors [ idx ] = new Sector ( GetSectorSize (), sourceStream ) { Id = idx };
							}

							freeList.Enqueue ( sectors [ idx ] as Sector );
						}

						idx++;
					}
				}
				else
				{
					List<Sector> miniFAT = GetSectorChain ( header.FirstMiniFATSectorID, SectorType.Normal );
					StreamView miniFATView = new StreamView ( miniFAT, GetSectorSize (), header.MiniFATSectorsNumber * Sector.MINISECTOR_SIZE, null, sourceStream );
					List<Sector> miniStream = GetSectorChain ( RootEntry.StartSetc, SectorType.Normal );
					StreamView miniStreamView = new StreamView ( miniStream, GetSectorSize (), rootStorage.Size, null, sourceStream );

					int idx = 0;

					int nMinisectors = ( int ) ( miniStreamView.Length / Sector.MINISECTOR_SIZE );

					while ( idx < nMinisectors )
					{
						//AssureLength(miniStreamView, (int)miniFATView.Length);

						int nextId = miniFATView.ReadInt32 ();

						if ( nextId == Sector.FREESECT )
						{
							Sector ms = new Sector ( Sector.MINISECTOR_SIZE, sourceStream );
							byte [] temp = new byte [ Sector.MINISECTOR_SIZE ];

							ms.Id = idx;
							ms.Type = SectorType.Mini;

							miniStreamView.Seek ( ms.Id * Sector.MINISECTOR_SIZE, SeekOrigin.Begin );
							miniStreamView.Read ( ms.GetData (), 0, Sector.MINISECTOR_SIZE );

							freeList.Enqueue ( ms );
						}

						idx++;
					}
				}

				return freeList;
			}

			internal void SetStreamLength ( CFItem cfItem, long length )
			{
				if ( cfItem.Size == length )
					return;

				SectorType newSectorType = SectorType.Normal;
				int newSectorSize = GetSectorSize ();

				if ( length < header.MinSizeStandardStream )
				{
					newSectorType = SectorType.Mini;
					newSectorSize = Sector.MINISECTOR_SIZE;
				}

				SectorType oldSectorType = SectorType.Normal;
				int oldSectorSize = GetSectorSize ();

				if ( cfItem.Size < header.MinSizeStandardStream )
				{
					oldSectorType = SectorType.Mini;
					oldSectorSize = Sector.MINISECTOR_SIZE;
				}

				long oldSize = cfItem.Size;


				// Get Sector chain and delta size induced by client
				List<Sector> sectorChain = GetSectorChain ( cfItem.DirEntry.StartSetc, oldSectorType );
				long delta = length - cfItem.Size;

				bool transitionToMini = false;
				bool transitionToNormal = false;
				List<Sector> oldChain = null;

				if ( cfItem.DirEntry.StartSetc != Sector.ENDOFCHAIN )
				{
					if (
						( length < header.MinSizeStandardStream && cfItem.DirEntry.Size >= header.MinSizeStandardStream )
						|| ( length >= header.MinSizeStandardStream && cfItem.DirEntry.Size < header.MinSizeStandardStream )
					   )
					{
						if ( cfItem.DirEntry.Size < header.MinSizeStandardStream )
						{
							transitionToNormal = true;
							oldChain = sectorChain;
						}
						else
						{
							transitionToMini = true;
							oldChain = sectorChain;
						}
					}
				}


				Queue<Sector> freeList = null;
				StreamView sv = null;

				if ( !transitionToMini && !transitionToNormal )   //############  NO TRANSITION
				{
					if ( delta > 0 ) // Enlarging stream...
					{
						if ( this.sectorRecycle )
							freeList = FindFreeSectors ( newSectorType ); // Collect available free sectors

						sv = new StreamView ( sectorChain, newSectorSize, length, freeList, sourceStream );

						//Set up  destination chain
						SetSectorChain ( sectorChain );
					}
					else if ( delta < 0 )  // Reducing size...
					{

						int nSec = ( int ) Math.Floor ( ( ( double ) ( Math.Abs ( delta ) ) / newSectorSize ) ); //number of sectors to mark as free

						if ( newSectorSize == Sector.MINISECTOR_SIZE )
							FreeMiniChain ( sectorChain, nSec, this.eraseFreeSectors );
						else
							FreeChain ( sectorChain, nSec, this.eraseFreeSectors );
					}

					if ( sectorChain.Count > 0 )
					{
						cfItem.DirEntry.StartSetc = sectorChain [ 0 ].Id;
						cfItem.DirEntry.Size = length;
					}
					else
					{
						cfItem.DirEntry.StartSetc = Sector.ENDOFCHAIN;
						cfItem.DirEntry.Size = 0;
					}

				}
				else if ( transitionToMini )                          //############## TRANSITION TO MINISTREAM
				{
					// Transition Normal chain -> Mini chain

					// Collect available MINI free sectors

					if ( this.sectorRecycle )
						freeList = FindFreeSectors ( SectorType.Mini );

					sv = new StreamView ( oldChain, oldSectorSize, oldSize, null, sourceStream );

					// Reset start sector and size of dir entry
					cfItem.DirEntry.StartSetc = Sector.ENDOFCHAIN;
					cfItem.DirEntry.Size = 0;

					List<Sector> newChain = GetMiniSectorChain ( Sector.ENDOFCHAIN );
					StreamView destSv = new StreamView ( newChain, Sector.MINISECTOR_SIZE, length, freeList, sourceStream );

					// Buffered trimmed copy from old (larger) to new (smaller)
					int cnt = 4096 < length ? 4096 : ( int ) length;

					byte [] buf = new byte [ 4096 ];
					long toRead = length;

					//Copy old to new chain
					while ( toRead > cnt )
					{
						cnt = sv.Read ( buf, 0, cnt );
						toRead -= cnt;
						destSv.Write ( buf, 0, cnt );
					}

					sv.Read ( buf, 0, ( int ) toRead );
					destSv.Write ( buf, 0, ( int ) toRead );

					//Free old chain
					FreeChain ( oldChain, this.eraseFreeSectors );

					//Set up destination chain
					AllocateMiniSectorChain ( destSv.BaseSectorChain );

					// Persist to normal strea
					PersistMiniStreamToStream ( destSv.BaseSectorChain );

					//Update dir item
					if ( destSv.BaseSectorChain.Count > 0 )
					{
						cfItem.DirEntry.StartSetc = destSv.BaseSectorChain [ 0 ].Id;
						cfItem.DirEntry.Size = length;
					}
					else
					{
						cfItem.DirEntry.StartSetc = Sector.ENDOFCHAIN;
						cfItem.DirEntry.Size = 0;
					}
				}
				else if ( transitionToNormal )                        //############## TRANSITION TO NORMAL STREAM
				{
					// Transition Mini chain -> Normal chain

					if ( this.sectorRecycle )
						freeList = FindFreeSectors ( SectorType.Normal ); // Collect available Normal free sectors

					sv = new StreamView ( oldChain, oldSectorSize, oldSize, null, sourceStream );

					List<Sector> newChain = GetNormalSectorChain ( Sector.ENDOFCHAIN );
					StreamView destSv = new StreamView ( newChain, GetSectorSize (), length, freeList, sourceStream );

					int cnt = 256 < length ? 256 : ( int ) length;

					byte [] buf = new byte [ 256 ];
					long toRead = Math.Min ( length, cfItem.Size );

					//Copy old to new chain
					while ( toRead > cnt )
					{
						cnt = sv.Read ( buf, 0, cnt );
						toRead -= cnt;
						destSv.Write ( buf, 0, cnt );
					}

					sv.Read ( buf, 0, ( int ) toRead );
					destSv.Write ( buf, 0, ( int ) toRead );

					//Free old mini chain
					int oldChainCount = oldChain.Count;
					FreeMiniChain ( oldChain, this.eraseFreeSectors );

					//Set up normal destination chain
					AllocateSectorChain ( destSv.BaseSectorChain );

					//Update dir item
					if ( destSv.BaseSectorChain.Count > 0 )
					{
						cfItem.DirEntry.StartSetc = destSv.BaseSectorChain [ 0 ].Id;
						cfItem.DirEntry.Size = length;
					}
					else
					{
						cfItem.DirEntry.StartSetc = Sector.ENDOFCHAIN;
						cfItem.DirEntry.Size = 0;
					}
				}
			}

			private void CheckFileLength ()
			{
				throw new NotImplementedException ();
			}

			internal int ReadData ( CFStream cFStream, long position, byte [] buffer, int count )
			{
				if ( count > buffer.Length )
					throw new ArgumentException ( "count parameter exceeds buffer size" );

				IDirectoryEntry de = cFStream.DirEntry;

				count = ( int ) Math.Min ( ( long ) ( de.Size - position ), ( long ) count );

				StreamView sView = null;
				if ( de.Size < header.MinSizeStandardStream )
					sView = new StreamView ( GetSectorChain ( de.StartSetc, SectorType.Mini ), Sector.MINISECTOR_SIZE, de.Size, null, sourceStream );
				else
					sView = new StreamView ( GetSectorChain ( de.StartSetc, SectorType.Normal ), GetSectorSize (), de.Size, null, sourceStream );

				sView.Seek ( position, SeekOrigin.Begin );
				int result = sView.Read ( buffer, 0, count );

				return result;
			}

			internal int ReadData ( CFStream cFStream, long position, byte [] buffer, int offset, int count )
			{

				IDirectoryEntry de = cFStream.DirEntry;

				count = ( int ) Math.Min ( ( long ) ( de.Size - offset ), ( long ) count );

				StreamView sView = null;


				if ( de.Size < header.MinSizeStandardStream )
				{
					sView
						= new StreamView ( GetSectorChain ( de.StartSetc, SectorType.Mini ), Sector.MINISECTOR_SIZE, de.Size, null, sourceStream );
				}
				else
				{

					sView = new StreamView ( GetSectorChain ( de.StartSetc, SectorType.Normal ), GetSectorSize (), de.Size, null, sourceStream );
				}


				sView.Seek ( position, SeekOrigin.Begin );
				int result = sView.Read ( buffer, offset, count );

				return result;
			}


			internal byte [] GetData ( CFStream cFStream )
			{

				if ( _disposed )
					throw new CFDisposedException ( "Compound File closed: cannot access data" );

				byte [] result = null;

				IDirectoryEntry de = cFStream.DirEntry;

				if ( de.Size < header.MinSizeStandardStream )
				{

					StreamView miniView
						= new StreamView ( GetSectorChain ( de.StartSetc, SectorType.Mini ), Sector.MINISECTOR_SIZE, de.Size, null, sourceStream );

					BinaryReader br = new BinaryReader ( miniView );

					result = br.ReadBytes ( ( int ) de.Size );
					br.Dispose ();

				}
				else
				{
					StreamView sView
						= new StreamView ( GetSectorChain ( de.StartSetc, SectorType.Normal ), GetSectorSize (), de.Size, null, sourceStream );

					result = new byte [ ( int ) de.Size ];

					sView.Read ( result, 0, result.Length );

				}

				return result;
			}
			public byte [] GetDataBySID ( int sid )
			{
				if ( _disposed )
					throw new CFDisposedException ( "Compound File closed: cannot access data" );
				if ( sid < 0 )
					return null;
				byte [] result = null;
				try
				{
					IDirectoryEntry de = directoryEntries [ sid ];
					if ( de.Size < header.MinSizeStandardStream )
					{
						StreamView miniView
							= new StreamView ( GetSectorChain ( de.StartSetc, SectorType.Mini ), Sector.MINISECTOR_SIZE, de.Size, null, sourceStream );
						BinaryReader br = new BinaryReader ( miniView );
						result = br.ReadBytes ( ( int ) de.Size );
						br.Dispose ();
					}
					else
					{
						StreamView sView
							= new StreamView ( GetSectorChain ( de.StartSetc, SectorType.Normal ), GetSectorSize (), de.Size, null, sourceStream );
						result = new byte [ ( int ) de.Size ];
						sView.Read ( result, 0, result.Length );
					}
				}
				catch
				{
					throw new CFException ( "Cannot get data for SID" );
				}
				return result;
			}
			public Guid GetGuidBySID ( int sid )
			{
				if ( _disposed )
					throw new CFDisposedException ( "Compound File closed: cannot access data" );
				if ( sid < 0 )
					throw new CFException ( "Invalid SID" );
				IDirectoryEntry de = directoryEntries [ sid ];
				return de.StorageCLSID;
			}
			public Guid GetGuidForStream ( int sid )
			{
				if ( _disposed )
					throw new CFDisposedException ( "Compound File closed: cannot access data" );
				if ( sid < 0 )
					throw new CFException ( "Invalid SID" );
				Guid g = new Guid ( "00000000000000000000000000000000" );
				//find first storage containing a non-zero CLSID before SID in directory structure
				for ( int i = sid - 1; i >= 0; i-- )
				{
					if ( directoryEntries [ i ].StorageCLSID != g && directoryEntries [ i ].StgType == StgType.StgStorage )
					{
						return directoryEntries [ i ].StorageCLSID;
					}
				}
				return g;
			}

			private static int Ceiling ( double d )
			{
				return ( int ) Math.Ceiling ( d );
			}

			private static int LowSaturation ( int i )
			{
				return i > 0 ? i : 0;
			}

			internal void FreeAssociatedData ( int sid )
			{
				// Clear the associated stream (or ministream) if required
				if ( directoryEntries [ sid ].Size > 0 ) //thanks to Mark Bosold for this !
				{
					if ( directoryEntries [ sid ].Size < header.MinSizeStandardStream )
					{
						List<Sector> miniChain
							= GetSectorChain ( directoryEntries [ sid ].StartSetc, SectorType.Mini );
						FreeMiniChain ( miniChain, this.eraseFreeSectors );
					}
					else
					{
						List<Sector> chain
							= GetSectorChain ( directoryEntries [ sid ].StartSetc, SectorType.Normal );
						FreeChain ( chain, this.eraseFreeSectors );
					}
				}
			}

			private bool closeStream = true;

			#region IDisposable Members

			private bool _disposed;//false

			public void Dispose ()
			{
				Dispose ( true );
				GC.SuppressFinalize ( this );
			}

			#endregion

			private object lockObject = new Object ();

			protected virtual void Dispose ( bool disposing )
			{
				try
				{
					if ( !_disposed )
					{
						lock ( lockObject )
						{
							if ( disposing )
							{
								// Call from user code...

								if ( sectors != null )
								{
									sectors.Clear ();
									sectors = null;
								}

								this.rootStorage = null; // Some problem releasing resources...
								this.header = null;
								this.directoryEntries.Clear ();
								this.directoryEntries = null;
								this.fileName = null;
								//this.lockObject = null;
							}

							if ( this.sourceStream != null && closeStream && !configuration.HasFlag ( CFSConfiguration.LeaveOpen ) )
								this.sourceStream.Dispose ();
						}
					}
				}
				finally
				{
					_disposed = true;
				}

			}

			internal bool IsClosed
			{
				get
				{
					return _disposed;
				}
			}

			private List<IDirectoryEntry> directoryEntries
				= new List<IDirectoryEntry> ();

			internal IList<IDirectoryEntry> GetDirectories ()
			{
				return directoryEntries;
			}

			internal IDirectoryEntry RootEntry
			{
				get
				{
					return directoryEntries [ 0 ];
				}
			}

			private IList<IDirectoryEntry> FindDirectoryEntries ( String entryName )
			{
				List<IDirectoryEntry> result = new List<IDirectoryEntry> ();

				foreach ( IDirectoryEntry d in directoryEntries )
				{
					if ( d.GetEntryName () == entryName && d.StgType != StgType.StgInvalid )
						result.Add ( d );
				}

				return result;
			}

			public IList<CFItem> GetAllNamedEntries ( String entryName )
			{
				IList<IDirectoryEntry> r = FindDirectoryEntries ( entryName );
				List<CFItem> result = new List<CFItem> ();

				foreach ( IDirectoryEntry id in r )
					if ( id.GetEntryName () == entryName && id.StgType != StgType.StgInvalid )
						result.Add ( id.StgType == StgType.StgStorage ? ( CFItem ) new CFStorage ( this, id ) : ( CFItem ) new CFStream ( this, id ) );

				return result;
			}

			public int GetNumDirectories ()
			{
				if ( _disposed )
					throw new CFDisposedException ( "Compound File closed: cannot access data" );
				return directoryEntries.Count;
			}

			public string GetNameDirEntry ( int id )
			{
				if ( _disposed )
					throw new CFDisposedException ( "Compound File closed: cannot access data" );
				if ( id < 0 )
					throw new CFException ( "Invalid Storage ID" );
				return directoryEntries [ id ].Name;
			}

			public StgType GetStorageType ( int id )
			{
				if ( _disposed )
					throw new CFDisposedException ( "Compound File closed: cannot access data" );
				if ( id < 0 )
					throw new CFException ( "Invalid Storage ID" );
				return directoryEntries [ id ].StgType;
			}
		}
		#endregion
	}
}
