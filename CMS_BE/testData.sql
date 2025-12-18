use master;
ALTER DATABASE CMSDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE CMSDB;

use CMSDB;

-- Insert Admin User
INSERT INTO Users (UserID, Name, Email, PhoneNumber, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
('11111111-1111-1111-1111-111111111111', 'Admin User', 'admin@cms.com', 1234567890, 'hashed_password_admin', 1, 1, GETUTCDATE(), GETUTCDATE(), 0);

-- Insert Patient User
INSERT INTO Users (UserID, Name, Email, PhoneNumber, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
('22222222-2222-2222-2222-222222222222', 'John Patient', 'patient@cms.com', 9876543210, 'hashed_password_patient', 4, 1, GETUTCDATE(), GETUTCDATE(), 0);

-- Insert Doctor User
INSERT INTO Users (UserID, Name, Email, PhoneNumber, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
('33333333-3333-3333-3333-333333333333', 'Dr. Smith', 'doctor@cms.com', 5551234567, 'hashed_password_doctor', 2, 1, GETUTCDATE(), GETUTCDATE(), 0);

-- Insert Staff User
INSERT INTO Users (UserID, Name, Email, PhoneNumber, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
('44444444-4444-4444-4444-444444444444', 'Jane Staff', 'staff@cms.com', 5559876543, 'hashed_password_staff', 3, 1, GETUTCDATE(), GETUTCDATE(), 0);

-- Insert Additional Doctor Users for Testing
INSERT INTO Users (UserID, Name, Email, PhoneNumber, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt, IsDeleted)
VALUES 
('55555555-5555-5555-5555-555555555555', 'Dr. Johnson', 'johnson@cms.com', 5551111111, 'hashed_password_johnson', 2, 1, GETUTCDATE(), GETUTCDATE(), 0),
('66666666-6666-6666-6666-666666666666', 'Dr. Williams', 'williams@cms.com', 5552222222, 'hashed_password_williams', 2, 1, GETUTCDATE(), GETUTCDATE(), 0),
('77777777-7777-7777-7777-777777777777', 'Dr. Brown', 'brown@cms.com', 5553333333, 'hashed_password_brown', 2, 1, GETUTCDATE(), GETUTCDATE(), 0);

-- Insert Doctor Details
-- Doctor 1: Normal availability (Dr. Smith - Cardiology)
INSERT INTO Doctors (DoctorID, Specialization, Qualification, YearOfExperience, WorkingDays, StartTime, EndTime, SlotDuration, BreakStartTime, BreakEndTime, IsDeleted)
VALUES 
('33333333-3333-3333-3333-333333333333', 'Cardiology', 'MBBS, MD Cardiology', 10, '[1,2,3,4,5]', '09:00:00', '17:00:00', 30, '13:00:00', '14:00:00', 0);

-- Doctor 2: With full-day leave (Dr. Johnson - Neurology)
INSERT INTO Doctors (DoctorID, Specialization, Qualification, YearOfExperience, WorkingDays, StartTime, EndTime, SlotDuration, BreakStartTime, BreakEndTime, IsDeleted)
VALUES 
('55555555-5555-5555-5555-555555555555', 'Neurology', 'MBBS, MD Neurology', 8, '[1,2,3,4,5]', '10:00:00', '18:00:00', 30, '14:00:00', '15:00:00', 0);

-- Doctor 3: With partial leave and booked slots (Dr. Williams - Orthopedics)
INSERT INTO Doctors (DoctorID, Specialization, Qualification, YearOfExperience, WorkingDays, StartTime, EndTime, SlotDuration, BreakStartTime, BreakEndTime, IsDeleted)
VALUES 
('66666666-6666-6666-6666-666666666666', 'Orthopedics', 'MBBS, MS Orthopedics', 12, '[1,2,3,4,5,6]', '08:00:00', '16:00:00', 30, '12:00:00', '13:00:00', 0);

-- Doctor 4: With full-day APPROVED leave (Dr. Brown - Dermatology)
INSERT INTO Doctors (DoctorID, Specialization, Qualification, YearOfExperience, WorkingDays, StartTime, EndTime, SlotDuration, BreakStartTime, BreakEndTime, IsDeleted)
VALUES 
('77777777-7777-7777-7777-777777777777', 'Dermatology', 'MBBS, MD Dermatology', 15, '[1,2,3,4,5]', '09:00:00', '18:00:00', 30, '13:00:00', '14:00:00', 0);

-- Insert Patient for testing
INSERT INTO Patients (PatientID, DateOfBirth, Sex, Address, BloodGroup, Allergies, IsDeleted)
VALUES 
('22222222-2222-2222-2222-222222222222', '1990-05-15', 1, '123 Main St, City', 'O+', 'None', 0);

-- SCENARIO 1: Dr. Smith (Normal availability) - No leaves, no bookings
-- This doctor should show all available slots except break time

-- SCENARIO 2: Dr. Johnson (Full-day leave) - Full day leave tomorrow (PENDING - will show slots)
INSERT INTO Leaves (LeaveID, UserID, StartDate, EndDate, Reason, Status, LeaveApplied, IsFullDay, IsDeleted)
VALUES 
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '55555555-5555-5555-5555-555555555555', 
 DATEADD(day, 1, CAST(GETDATE() AS DATE)), DATEADD(day, 1, CAST(GETDATE() AS DATE)), 
 'Medical Conference', 2, GETUTCDATE(), 1, 0);

-- SCENARIO 3: Dr. Williams (Partial leave + booked slots)
-- Partial leave from 10:00 AM to 12:00 PM tomorrow (APPROVED)
INSERT INTO Leaves (LeaveID, UserID, StartDate, EndDate, Reason, Status, LeaveApplied, IsFullDay, IsDeleted)
VALUES 
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '66666666-6666-6666-6666-666666666666', 
 CAST(DATEADD(day, 1, CAST(GETDATE() AS DATE)) AS DATETIME) + CAST('10:00:00' AS DATETIME), 
 CAST(DATEADD(day, 1, CAST(GETDATE() AS DATE)) AS DATETIME) + CAST('12:00:00' AS DATETIME), 
 'Personal Work', 1, GETUTCDATE(), 0, 0);

-- SCENARIO 4: Dr. Brown (Full-day APPROVED leave) - Should return 0 slots
INSERT INTO Leaves (LeaveID, UserID, StartDate, EndDate, Reason, Status, LeaveApplied, IsFullDay, IsDeleted)
VALUES 
('ffffffff-ffff-ffff-ffff-ffffffffffff', '77777777-7777-7777-7777-777777777777', 
 DATEADD(day, 1, CAST(GETDATE() AS DATE)), DATEADD(day, 1, CAST(GETDATE() AS DATE)), 
 'Medical Conference', 1, GETUTCDATE(), 1, 0);

-- Wait for all doctors to be inserted, then add TimeSlots
-- Booked time slots for Dr. Williams tomorrow (morning and afternoon slots)
INSERT INTO TimeSlots (SlotID, DoctorID, SlotDate, StartTime, EndTime, IsAvailable, CreatedAt, IsDeleted)
VALUES 
('cccccccc-cccc-cccc-cccc-cccccccccccc', '66666666-6666-6666-6666-666666666666', 
 DATEADD(day, 1, CAST(GETDATE() AS DATE)), '08:00:00', '08:30:00', 0, GETUTCDATE(), 0),
('dddddddd-dddd-dddd-dddd-dddddddddddd', '66666666-6666-6666-6666-666666666666', 
 DATEADD(day, 1, CAST(GETDATE() AS DATE)), '08:30:00', '09:00:00', 0, GETUTCDATE(), 0),
('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '66666666-6666-6666-6666-666666666666', 
 DATEADD(day, 1, CAST(GETDATE() AS DATE)), '14:00:00', '14:30:00', 0, GETUTCDATE(), 0);

select * from Users;
select * from Doctors;
select * from Patients;
select * from Leaves;
select * from TimeSlots;

-- Test Queries to verify data
PRINT 'Tomorrow''s date: ' + CAST(DATEADD(day, 1, CAST(GETDATE() AS DATE)) AS VARCHAR);
PRINT 'Dr. Smith ID (Normal): 33333333-3333-3333-3333-333333333333';
PRINT 'Dr. Johnson ID (Pending Leave - Shows Slots): 55555555-5555-5555-5555-555555555555';
PRINT 'Dr. Williams ID (Partial Approved Leave + Bookings): 66666666-6666-6666-6666-666666666666';
PRINT 'Dr. Brown ID (Full-Day Approved Leave - 0 Slots): 77777777-7777-7777-7777-777777777777';
