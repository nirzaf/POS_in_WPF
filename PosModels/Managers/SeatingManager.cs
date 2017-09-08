using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using PosModels.Types;

namespace PosModels.Managers
{
    /// <summary>
    /// Seating and Room instance manager. Only maintains instances of Seating and Room
    /// that have IsUnused set to FALSE.
    /// </summary>
    public static class SeatingManager
    {
        private static Dictionary<int, Seating> Seatings = new Dictionary<int, Seating>();
        private static Dictionary<int, Room> Rooms = new Dictionary<int, Room>();

        static SeatingManager()
        {
            UpdateSeatings();
            UpdateRooms();
        }

        private static void UpdateRooms()
        {
            Rooms.Clear();
            IEnumerable<Room> rooms = Room.GetAll(false);
            foreach (Room room in rooms)
            {
                Rooms.Add(room.Id, room);
            }
        }

        private static void UpdateSeatings()
        {
            Seatings.Clear();
            IEnumerable<Seating> seatings = Seating.GetAll(false);
            foreach (Seating seating in seatings)
            {
                Seatings.Add(seating.Id, seating);
            }
        }

        private static void NotifySeatingChanged()
        {            
            //SettingManager.SetStoreSetting(STORE_SETTING_SEATING_NAME, DateTime.Now);
        }

        private static void NotifyRoomsChanged()
        {
            //SettingManager.SetStoreSetting(STORE_SETTING_ROOMS_NAME, DateTime.Now);
        }

        public static Seating GetSeating(int seatingId)
        {
            if (!Seatings.Keys.Contains(seatingId))
                return null;
            return Seatings[seatingId];
        }

        public static Seating[] GetAllSeating()
        {
            return Seatings.Values.ToArray();
        }

        public static IEnumerable<Seating> GetAllSeating(int roomId)
        {
            foreach (Seating seating in Seatings.Values)
            {
                if (seating.RoomId == roomId)
                    yield return seating;
            }
        }

        public static Seating AddSeating(int roomId, string description, int capacity)
        {
            Seating newSeating = Seating.Add(roomId, description, capacity, false);
            Seatings.Add(newSeating.Id, newSeating);
            NotifySeatingChanged();
            return newSeating;
        }

        public static void DeleteSeating(int seatingId)
        {
            Seating seating = Seatings[seatingId];
            seating.SetIsUnused(true);
            seating.Update();
            Seatings.Remove(seatingId);
            NotifySeatingChanged();
        }

        public static void DeleteAllSeating(int roomId)
        {
            IEnumerable<Seating> seatings = GetAllSeating(roomId);
            foreach (Seating seating in seatings)
            {
                seating.SetIsUnused(true);
                seating.Update();
                Seatings.Remove(seating.Id);
            }
            NotifySeatingChanged();
        }

        public static bool UpdateSeating(Seating seating)
        {
            bool result = Seating.Update(seating);
            if (result)
                NotifySeatingChanged();
            return result;
        }

        public static Room GetRoom(int roomId)
        {
            if (!Rooms.Keys.Contains(roomId))
                return null;
            return Rooms[roomId];
        }

        public static Room[] GetAllRoom()
        {
            return Rooms.Values.ToArray();
        }

        public static Room AddRoom(string description, TicketType ticketType)
        {
            Room newRoom = Room.Add(description, ticketType, false);
            Rooms.Add(newRoom.Id, newRoom);
            NotifyRoomsChanged();
            return newRoom;
        }

        public static void DeleteRoom(int roomId)
        {
            Room room = Rooms[roomId];
            room.SetIsUnused(true);
            room.Update();
            Rooms.Remove(roomId);
            NotifyRoomsChanged();
        }

        public static bool UpdateRoom(Room room)
        {
            bool result = Room.Update(room);
            if (result)
                NotifyRoomsChanged();
            return result;
        }

    }
}
