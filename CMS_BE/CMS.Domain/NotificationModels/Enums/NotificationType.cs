namespace CMS.Domain.NotificationModels.Enums
{
    public enum NotificationType
    {
        // Appointment related
        AppointmentScheduled = 1,
        AppointmentReminder = 2,
        AppointmentCancelled = 3,
        AppointmentRescheduled = 4,
        AppointmentConfirmed = 5,
        
        // Emergency and Priority
        EmergencyAlert = 10,
        RedZoneAlert = 11,
        OrangeZoneAlert = 12,
        GreenZoneAlert = 13,
        
        // Billing and Payment
        PaymentReminder = 20,
        PaymentReceived = 21,
        PaymentFailed = 22,
        InvoiceGenerated = 23,
        PaymentOverdue = 24,
        
        // Medical Records
        LabResultReady = 30,
        PrescriptionReady = 31,
        TestResultAvailable = 32,
        ScanResultAvailable = 33,
        MedicalReportGenerated = 34,
        
        // Doctor Visits
        DoctorVisitScheduled = 40,
        DoctorVisitCompleted = 41,
        PrescriptionUpdated = 42,
        LabTestOrdered = 43,
        ScanOrdered = 44,
        
        // Follow-up
        FollowUpReminder = 50,
        FollowUpScheduled = 51,
        FollowUpOverdue = 52,
        
        // System
        SystemMaintenance = 90,
        SecurityAlert = 91,
        DataBackup = 92,
        
        // General
        General = 100,
        Welcome = 101,
        PasswordReset = 102,
        AccountActivated = 103,
        AccountSuspended = 104
    }
}
