using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PosModels;

namespace TemPOS.Helpers
{
    public class UnitedStatesCity
    {
        public string City
        {
            get;
            private set;
        }

        public string State
        {
            get;
            private set;
        }

        public string StateAbbreviation
        {
            get;
            private set;
        }

        public int[] PostalCodes
        {
            get;
            private set;
        }

        private UnitedStatesCity(int[] postalCodes, string city, string state, string stateAbbreviation)
        {
            PostalCodes = postalCodes;
            City = city;
            State = state;
            StateAbbreviation = stateAbbreviation;
        }

        public static UnitedStatesCity Find(int zipCode)
        {
            ZipCode zipcode = ZipCode.Get(zipCode);
            ZipCodeCity city = ZipCodeCity.Get(zipcode.CityId);
            ZipCodeState state = ZipCodeState.Get(city.StateId);
            IEnumerable<ZipCode> allCityZipCodes = ZipCode.GetAll(zipcode.CityId);
            int[] allIntCityZipCodes = ToIntArray(allCityZipCodes);
            return new UnitedStatesCity(allIntCityZipCodes, city.City, state.StateName, state.Abbreviation);
        }

        public static UnitedStatesCity Find(string cityName, string stateName)
        {
            ZipCodeState state = ZipCodeState.GetByName(stateName);
            if (state == null)
                return null;
            ZipCodeCity city = ZipCodeCity.GetByName(state.Id, cityName);
            IEnumerable<ZipCode> allCityZipCodes = ZipCode.GetAll(city.Id);
            int[] allIntCityZipCodes = ToIntArray(allCityZipCodes);
            return new UnitedStatesCity(allIntCityZipCodes, city.City, state.StateName, state.Abbreviation);
        }

        private static int[] ToIntArray(IEnumerable<ZipCode> allCityZipCodes)
        {
            int index = -1;
            int[] allIntCityZipCodes = new int[allCityZipCodes.Count()];
            foreach (ZipCode zipCode in allCityZipCodes)
            {
                index++;
                allIntCityZipCodes[index] = zipCode.PostalCode;
            }
            if (index == -1)
                return null;
            return allIntCityZipCodes;
        }

    }
}
