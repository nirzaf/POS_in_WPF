using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Managers
{
    public class PhoneNumberManager
    {
        private static Dictionary<int, PhoneNumber> PhoneNumbers = new Dictionary<int, PhoneNumber>();

        static PhoneNumberManager()
        {
            IEnumerable<PhoneNumber> tickets = PhoneNumber.GetAll();
            foreach (PhoneNumber ticket in tickets)
            {
                PhoneNumbers.Add(ticket.Id, ticket);
            }
        }

        /// <summary>
        /// Add a new entry to the PhoneNumber table
        /// </summary>
        public static PhoneNumber Add(string phoneNumber, string description)
        {
            PhoneNumber newPhoneNumber = PhoneNumber.Add(phoneNumber, description);
            PhoneNumbers.Add(newPhoneNumber.Id, newPhoneNumber);
            return newPhoneNumber;
        }

        /// <summary>
        /// Delete a PhoneNumber table entry
        /// </summary>
        public static bool Delete(int phoneNumberId)
        {
            // Scan existing
            if (PhoneNumbers.Keys.Contains(phoneNumberId))
            {
                PhoneNumbers.Remove(phoneNumberId);
            }
            return PhoneNumber.Delete(phoneNumberId);
        }

        /// <summary>
        /// Get a single PhoneNumber from the PhoneNumber table
        /// </summary>
        public static PhoneNumber GetPhoneNumber(int phoneNumberId)
        {
            // Invalid Id
            if (phoneNumberId <= 0)
                return null;

            // Scan existing
            if (PhoneNumbers.Keys.Contains(phoneNumberId))
            {
                PhoneNumber.Refresh(PhoneNumbers[phoneNumberId]);
                return PhoneNumbers[phoneNumberId];
            }

            // Not found, let's check the database
            PhoneNumber newPhoneNumber = PhoneNumber.Get(phoneNumberId);
            if (newPhoneNumber != null)
            {
                PhoneNumbers.Add(newPhoneNumber.Id, newPhoneNumber);
                return newPhoneNumber;
            }
            return null;
        }

        /// <summary>
        /// Get all people in the PhoneNumber table
        /// </summary>
        public static IEnumerable<PhoneNumber> GetAllPhoneNumbers()
        {
            IEnumerable<PhoneNumber> currentPhoneNumbers = PhoneNumber.GetAll();
            foreach (PhoneNumber ticket in currentPhoneNumbers)
            {
                yield return GetUpdatedManagedPhoneNumber(ticket);
            }
        }

        /// <summary>
        /// Gets the managed PhoneNumber, or creates a managed PhoneNumber if
        /// one doesn't exist.
        /// </summary>
        private static PhoneNumber GetUpdatedManagedPhoneNumber(PhoneNumber phoneNumber)
        {
            if (!PhoneNumbers.Keys.Contains(phoneNumber.Id))
            {
                // PhoneNumber is not a managed instance yet
                PhoneNumbers.Add(phoneNumber.Id, phoneNumber);
            }
            else
            {
                // Refresh the managed PhoneNumber with the current phoneNumber instance
                PhoneNumber.Refresh(PhoneNumbers[phoneNumber.Id], phoneNumber);
            }
            return PhoneNumbers[phoneNumber.Id];
        }
    }
}
