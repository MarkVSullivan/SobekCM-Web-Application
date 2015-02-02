using System.Collections.Generic;
using System.Linq;

namespace SobekCM.Engine_Library.IpRangeUtilities
{
    public class IpRangeSetV4
    {
        private readonly List<SingleIpRangeV4> ranges;
        private Dictionary<byte, IList<SingleIpRangeV4>> prefixDictionary;

        public IpRangeSetV4()
        {
            ranges = new List<SingleIpRangeV4>();
        }

        public void AddIpRange(SingleIpRangeV4 IpRange )
        {
            ranges.Add(IpRange);
            prefixDictionary = null;
        }

        public void AddIpRange(ulong SingleIpAddress)
        {
            SingleIpRangeV4 range = new SingleIpRangeV4(SingleIpAddress);
            AddIpRange(range);
        }

        public void AddIpRange(ulong StartIpAddress, ulong EndIpAddress)
        {
            SingleIpRangeV4 range = new SingleIpRangeV4(StartIpAddress, EndIpAddress);
            AddIpRange(range);
        }

        public void AddIpRange(string SingleIpAddress)
        {
            SingleIpRangeV4 range = new SingleIpRangeV4(SingleIpAddress);
            AddIpRange(range);
        }

        public void AddIpRange(string StartIpAddress, string EndIpAddress)
        {
            SingleIpRangeV4 range = new SingleIpRangeV4(StartIpAddress, EndIpAddress);
            AddIpRange(range);
        }

        public void Ready()
        {
            Dictionary<byte, SortedList<ulong, SingleIpRangeV4>> tempDictionary = new Dictionary<byte, SortedList<ulong, SingleIpRangeV4>>();
            foreach (SingleIpRangeV4 ipRange in ranges)
            {
                if (tempDictionary.ContainsKey(ipRange.Prefix))
                {
                    tempDictionary[ipRange.Prefix].Add(ipRange.StartIpAddress, ipRange);
                }
                else
                {
                    SortedList<ulong, SingleIpRangeV4> newSorted = new SortedList<ulong, SingleIpRangeV4> {{ipRange.StartIpAddress, ipRange}};
                    tempDictionary[ipRange.Prefix] = newSorted;
                }
            }

            prefixDictionary = new Dictionary<byte, IList<SingleIpRangeV4>>();
            foreach (byte thisKey in tempDictionary.Keys)
            {
                prefixDictionary[thisKey] = tempDictionary[thisKey].Values;
            }

        }

        public bool Contains(ComparableIpAddress Address)
        {
            // If not defined, or no IP ranges included, just return FALSE
            if ((ranges == null) || (ranges.Count == 0))
                return false;
            
            // Ensure the ranges have been pulled out
            if (prefixDictionary == null)
                Ready();

            // Look for a matching prefix
            if (!prefixDictionary.ContainsKey(Address.Prefix))
                return false;

            // Get the list
            IList<SingleIpRangeV4> values = prefixDictionary[Address.Prefix];

            // If a three ranges or less, do the comparison with the slow method and get out
            if (values.Count <= 3)
            {
                return values.Any(range => range.CompareTo(Address) == 0);
            }

            // Get the start and end indexes to get started
            int start_index = 0;
            int end_index = values.Count - 1;

            // While the start and index include three ranges ( start, end, and at least one between ) keep
            // checking for where on the list to check next, by dividing the list in half each time
            while (end_index - start_index >= 2)
            {
                // Find the new middle point
                int middle_point = (start_index + end_index)/2;

                // Perform the comparison to the middle range
                int comparison = values[middle_point].CompareTo(Address);

                // If the comparison shows the range holds the address, we found the fact it is contained!
                if (comparison == 0)
                    return true;

                // If the comparison is negative, than the range is less than the address
                if (comparison < 0)
                {
                    start_index = middle_point;
                }
                else  // The range is greater than the address
                {
                    end_index = middle_point;
                }
            }

            // Now, just step through the last couple and check for containment
            for (int i = start_index; i < end_index; i++)
            {
                if (values[i].CompareTo(Address) == 0)
                    return true;
            }

            return false;
        }
    }
}
