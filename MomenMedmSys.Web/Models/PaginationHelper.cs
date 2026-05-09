using System.Collections.Generic;
using System.Linq;

namespace MomenMedmSys.Web.Models
{
    public static class PaginationHelper
    {
        public static IEnumerable<int> GetPageNumbers(int currentPage, int totalPages, int maxVisible = 5)
        {
            if (totalPages <= maxVisible)
            {
                return Enumerable.Range(1, totalPages);
            }

            var pages = new List<int>();
            int start = Math.Max(1, currentPage - maxVisible / 2);
            int end = Math.Min(totalPages, start + maxVisible - 1);

            if (end - start + 1 < maxVisible)
            {
                start = Math.Max(1, end - maxVisible + 1);
            }

            if (start > 1)
            {
                pages.Add(1);
                if (start > 2) pages.Add(-1); // -1 represents "..."
            }

            for (int i = start; i <= end; i++)
            {
                pages.Add(i);
            }

            if (end < totalPages)
            {
                if (end < totalPages - 1) pages.Add(-1);
                pages.Add(totalPages);
            }

            return pages;
        }
    }
}
