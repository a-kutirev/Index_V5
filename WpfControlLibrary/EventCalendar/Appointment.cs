using System;

namespace WpfControlLibrary.EventCalendar
{
    public class Appointment
    {
        private int m_appointmentID;
        private string m_subject;
        private string m_location;
        private string m_details;
        private DateTime m_startTime;
        private DateTime m_endTime;
        private DateTime m_reccreatedDate;

        public Appointment()
        {

        }

        public int AppointmentID { get => m_appointmentID; set => m_appointmentID = value; }
        public string Subject { get => m_subject; set => m_subject = value; }
        public string Location { get => m_location; set => m_location = value; }
        public string Details { get => m_details; set => m_details = value; }
        public DateTime StartTime { get => m_startTime; set => m_startTime = value; }
        public DateTime EndTime { get => m_endTime; set => m_endTime = value; }
        public DateTime ReccreatedDate { get => m_reccreatedDate; set => m_reccreatedDate = value; }
    }
}
