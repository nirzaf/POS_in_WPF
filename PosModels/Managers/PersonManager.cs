using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PosModels.Managers
{
    public class PersonManager
    {
        private static Dictionary<int, Person> Persons = new Dictionary<int, Person>();

        static PersonManager()
        {
            IEnumerable<Person> tickets = Person.GetAll();
            foreach (Person ticket in tickets)
            {
                Persons.Add(ticket.Id, ticket);
            }
        }

        /// <summary>
        /// Add a new entry to the Person table
        /// </summary>
        public static Person Add(string firstName, char? middleInitial, string lastName,
            string address1, string address2, int zipCodeId, int phoneNumberId1,
            int phoneNumberId2,  int phoneNumberId3,  int phoneNumberId4,  int phoneNumberId5,
            int phoneNumberId6, string eMailAddress)
        {
            Person newPerson = Person.Add(firstName, middleInitial, lastName, address1, address2,
                zipCodeId, phoneNumberId1, phoneNumberId2, phoneNumberId3, phoneNumberId4,
                phoneNumberId5, phoneNumberId6, eMailAddress);
            Persons.Add(newPerson.Id, newPerson);
            return newPerson;
        }

        /// <summary>
        /// Delete a Person table entry
        /// </summary>
        public static bool Delete(int personId)
        {
            // Scan existing
            if (Persons.Keys.Contains(personId))
            {
                Persons.Remove(personId);
            }
            return Person.Delete(personId);
        }

        /// <summary>
        /// Get a single Person from the Person table
        /// </summary>
        public static Person GetPerson(int personId)
        {
            // Invalid Id
            if (personId <= 0)
                return null;

            // Scan existing
            if (Persons.Keys.Contains(personId))
            {
                Person.Refresh(Persons[personId]);
                return Persons[personId];
            }

            // Not found, let's check the database
            Person newPerson = Person.Get(personId);
            if (newPerson != null)
            {
                Persons.Add(newPerson.Id, newPerson);
                return newPerson;
            }
            return null;
        }

        public static Person GetPersonByEmployeeId(int employeeId)
        {
            int personId = Employee.GetPersonId(employeeId);
            return GetPerson(personId);
        }

        public static Person GetPersonByCustomerId(int customerId)
        {
            int personId = Customer.GetPersonId(customerId);
            return GetPerson(personId);
        }

        /// <summary>
        /// Get all people in the Person table
        /// </summary>
        public static IEnumerable<Person> GetAllPersons()
        {
            IEnumerable<Person> currentPersons = Person.GetAll();
            foreach (Person person in currentPersons)
            {
                yield return GetUpdatedManagedPerson(person);
            }
        }

        /// <summary>
        /// Gets the managed Person, or creates a managed Person if
        /// one doesn't exist.
        /// </summary>
        private static Person GetUpdatedManagedPerson(Person person)
        {
            if (!Persons.Keys.Contains(person.Id))
            {
                // Person is not a managed instance yet
                Persons.Add(person.Id, person);
            }
            else
            {
                // Refresh the managed Person with the current person instance
                Person.Refresh(Persons[person.Id], person);
            }
            return Persons[person.Id];
        }

    }
}
