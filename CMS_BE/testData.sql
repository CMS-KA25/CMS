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

-- Insert Doctor Details
INSERT INTO Doctors (DoctorID, Specialization, Qualification, YearOfExperience, WorkingDays, StartTime, EndTime, SlotDuration, BreakStartTime, BreakEndTime, IsDeleted)
VALUES 
('33333333-3333-3333-3333-333333333333', 'Cardiology', 'MBBS, MD Cardiology', 10, '[1,2,3,4,5]', '09:00:00', '17:00:00', 30, '13:00:00', '14:00:00', 0);

select * from Users;
select * from Doctors;