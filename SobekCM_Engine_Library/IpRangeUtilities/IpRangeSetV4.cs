#region Using directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace SobekCM.Engine_Library.IpRangeUtilities
{
    /// <summary> Set of IP address ranges used in the solution for quick IP range checks to
    /// support lookups within IP restrictions on the SobekCM engine </summary>
    /// <remarks>This supports very fast lookup to see if a provided IP address is within the
    /// set of IP ranges included within this set. </remarks>
    public class IpRangeSetV4
    {
        private readonly List<SingleIpRangeV4> ranges;
        private Dictionary<byte, IList<SingleIpRangeV4>> prefixDictionary;

        /// <summary> Constructor for a new instance of the IpRangeSetV4 class </summary>
        public IpRangeSetV4()
        {
            ranges = new List<SingleIpRangeV4>();
        }

        /// <summary> Add a single IP restriction range to this set </summary>
        /// <param name="IpRange"> Single IP range </param>
        public void AddIpRange(SingleIpRangeV4 IpRange )
        {
            ranges.Add(IpRange);
            prefixDictionary = null;
        }

        /// <summary> Add a single IP restriction range to this set </summary>
        /// <param name="SingleIpAddress"> Single IP address/range (as an unsigned long)</param>
        public void AddIpRange(ulong SingleIpAddress)
        {
            SingleIpRangeV4 range = new SingleIpRangeV4(SingleIpAddress);
            AddIpRange(range);
        }

        /// <summary> Add a single IP restriction range to this set </summary>
        /// <param name="StartIpAddress"> First IP address in the range (as an unsigned long)</param>
        /// <param name="EndIpAddress">  Last IP address in the range (as an unsigned long)</param>
        public void AddIpRange(ulong StartIpAddress, ulong EndIpAddress)
        {
            SingleIpRangeV4 range = new SingleIpRangeV4(StartIpAddress, EndIpAddress);
            AddIpRange(range);
        }

        /// <summary> Add a single IP restriction range to this set </summary>
        /// <param name="SingleIpAddress"> Single IP address/range (as a string)</param>
        public void AddIpRange(string SingleIpAddress)
        {
            SingleIpRangeV4 range = new SingleIpRangeV4(SingleIpAddress);
            AddIpRange(range);
        }

        /// <summary> Add a single IP restriction range to this set </summary>
        /// <param name="StartIpAddress"> First IP address in the range (as a string)</param>
        /// <param name="EndIpAddress">  Last IP address in the range (as a string)</param>
        public void AddIpRange(string StartIpAddress, string EndIpAddress)
        {
            SingleIpRangeV4 range = new SingleIpRangeV4(StartIpAddress, EndIpAddress);
            AddIpRange(range);
        }

        /// <summary> Readies this set for comparisons, by building some internal data structures </summary>
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

        /// <summary> Check to see if a given IP address is within the IP ranges in this set </summary>
        /// <param name="Address"> IP address to check for inclusion ( as a string )</param>
        /// <returns> TRUE if the IP address is within the ranges, otherwise FALSE </returns>
        public bool Contains(string Address)
        {
            ComparableIpAddress address = new ComparableIpAddress(Address);
            return Contains(address);
        }

        /// <summary> Check to see if a given IP address is within the IP ranges in this set </summary>
        /// <param name="Address"> IP address to check for inclusion ( as a <see cref="ComparableIpAddress" /> object )</param>
        /// <returns> TRUE if the IP address is within the ranges, otherwise FALSE </returns>
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
