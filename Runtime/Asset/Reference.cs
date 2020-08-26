

namespace LWFramework.Asset
{  
	public class Reference
	{
		public bool IsUnused ()
		{
			return refCount <= 0;
		}

		public int refCount;

		public void Retain ()
		{
			refCount++;
		}

		public void Release ()
		{
			refCount--;
		} 
	} 
}
