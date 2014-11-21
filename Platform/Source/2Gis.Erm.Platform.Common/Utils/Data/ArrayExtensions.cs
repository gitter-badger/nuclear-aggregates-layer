namespace DoubleGis.Erm.Platform.Common.Utils.Data
{
    public static class ArrayExtensions
    {
        public static bool SameAs(this byte[] first, byte[] second)
        {
            if (first == null || second == null)
            {
                return false;
            }

            if (first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
