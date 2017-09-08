using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PosModels.Types;

namespace PosModels.Managers
{
    public static class EmployeeManager
    {
        private static Dictionary<int, Employee> Employees = new Dictionary<int, Employee>();

        static EmployeeManager()
        {
            IEnumerable<Employee> employees = Employee.GetAll();
            foreach (Employee employee in employees)
            {
                Employees.Add(employee.Id, employee);
            }
        }

        /// <summary>
        /// Add a new entry to the Employee table
        /// </summary>
        public static Employee AddEmployee(int personId, DateTime hireDate,
            Permissions[] permissions, string password, string federalTaxId)
        {
            Employee newEmployee = Employee.Add(personId, permissions,
                password, federalTaxId);
            if (newEmployee != null)
            {
                EmployeeStatus.Add(newEmployee.Id, hireDate);
                Employees.Add(newEmployee.Id, newEmployee);
            }
            return newEmployee;
        }

        /// <summary>
        /// Add a new entry to the Employee table
        /// </summary>
        public static Employee AddEmployee(int personId, DateTime hireDate,
            Permissions[] permissions, byte[] scanCode, string federalTaxId)
        {
            Employee newEmployee = Employee.Add(personId,permissions,
                scanCode, federalTaxId);
            if (newEmployee != null)
            {
                EmployeeStatus.Add(newEmployee.Id, hireDate);
                Employees.Add(newEmployee.Id, newEmployee);
            }
            return newEmployee;
        }

        /// <summary>
        /// Removes an employee
        /// </summary>
        /// <param name="employee"></param>
        public static bool RemoveEmployee(Employee employee)
        {
            if (employee == null)
                return false;
            bool result = employee.Remove();
            if (Employees.Keys.Contains(employee.Id))
                Employees.Remove(employee.Id);
            return result;
        }

        /// <summary>
        /// Get a single employee from the Employee table
        /// </summary>
        public static Employee GetEmployee(int employeeId)
        {
            // Invalid Id
            if (employeeId <= 0)
                return null;

            // Scan existing
            if (Employees.Keys.Contains(employeeId))
            {
                Employee.Refresh(Employees[employeeId]);
                return Employees[employeeId];
            }

            // Not found, let's check the database
            Employee newEmployee = Employee.Get(employeeId);
            if (newEmployee != null)
            {
                Employees.Add(newEmployee.Id, newEmployee);
                return newEmployee;
            }
            return null;
        }

        /// <summary>
        /// Get all settings in the Employee table
        /// </summary>
        public static IEnumerable<Employee> GetAllEmployees()
        {
            IEnumerable<Employee> currentEmployees = Employee.GetAll();
            foreach (Employee ticket in currentEmployees)
            {
                yield return GetUpdatedManagedEmployee(ticket);
            }
        }

        public static Employee LookupByScanCode(string scanCode)
        {
            Employee temp = Employee.GetFromScanCode(scanCode);
            if (temp != null)
                return GetUpdatedManagedEmployee(temp);
            return null;
        }

        /// <summary>
        /// Gets the managed Employee, or creates a managed Employee if
        /// one doesn't exist.
        /// </summary>
        private static Employee GetUpdatedManagedEmployee(Employee employee)
        {
            if (!Employees.Keys.Contains(employee.Id))
            {
                // Employee is not a managed instance yet
                Employees.Add(employee.Id, employee);
            }
            else
            {
                // Refresh the managed employee with the current employee instance
                Employee.Refresh(Employees[employee.Id], employee);
            }
            return Employees[employee.Id];
        }

    }
}
