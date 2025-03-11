-- Create the Accommodation database
DROP DATABASE IF EXISTS Accommodation;
GO  
CREATE DATABASE Accommodation;
GO

-- Use the Accommodation database
USE Accommodation;
GO


-- Create Hotels table
DROP TABLE IF EXISTS Hotels;
GO
CREATE TABLE Hotels (
    -- Primary key identifier for the hotel
    Id INT IDENTITY(1,1) PRIMARY KEY,
    
    -- Name of the hotel (required)
    -- Examples: "Grand Hotel", "Seaside Resort"
    Name NVARCHAR(100) NOT NULL,
    
    -- Physical address of the hotel (required)
    -- Examples: "123 Main Street", "456 Ocean Drive"
    Address NVARCHAR(200) NOT NULL,
    
    -- City where the hotel is located (required)
    -- Examples: "New York", "Paris", "Tokyo"
    City NVARCHAR(100) NOT NULL,
    
    -- Country where the hotel is located (required)
    -- Examples: "United States", "France", "Japan"
    Country NVARCHAR(100) NOT NULL,
    
    -- Contact phone number for the hotel
    -- Format typically follows international standards: "+1-555-123-4567"
    PhoneNumber NVARCHAR(20) NULL,
    
    -- Contact email address for the hotel
    -- Examples: "info@grandhotel.com", "reservations@seasideresort.com"
    Email NVARCHAR(100) NULL,
    
    -- Status of the hotel record:
    -- 1 = Active (operational hotel)
    -- 2 = Inactive (temporarily not operational)
    -- 3 = DeletedForEveryone (soft deleted from all users)
    -- 4 = Pending (awaiting approval/verification)
    -- 5 = Archived (historical record, no longer in business)
    -- 6 = Suspended (suspended due to policy violations)
    -- 7 = DeletedForMe (soft deleted for specific users)
    EntityStatusId INT NOT NULL DEFAULT(1),
    
    -- Timestamp when the hotel record was created
    CreatedAt DATETIME NOT NULL DEFAULT(GETUTCDATE()),
    
    -- User ID of the person who created the hotel record
    CreatedBy INT NOT NULL,
    
    -- Timestamp when the hotel record was last updated
    UpdatedAt DATETIME NULL,
    
    -- User ID of the person who last updated the hotel record
    UpdatedBy INT  NULL
);

-- Add index for faster searching by name
CREATE INDEX IX_Hotels_Name ON Hotels(Name);

-- Add index for filtering by status
CREATE INDEX IX_Hotels_EntityStatusId ON Hotels(EntityStatusId);

-- Create UNIQUE index for preventing duplicate hotels at the same address
CREATE UNIQUE INDEX IX_Hotels_Name_Address ON Hotels(Name, Address) WHERE EntityStatusId = 1;

-- Insert 20 sample hotels
INSERT INTO Hotels (Name, Address, City, Country, PhoneNumber, Email, EntityStatusId, CreatedBy, UpdatedBy)
VALUES 
('Grand Hyatt', '123 Park Avenue', 'New York', 'United States', '+1-212-555-1234', 'reservations@grandhyatt.com', 1, 1, 1),
('Ritz-Carlton', '50 Central Park South', 'New York', 'United States', '+1-212-555-2345', 'info@ritzcarlton.com', 1, 1, 1),
('Burj Al Arab', 'Jumeirah Beach Road', 'Dubai', 'United Arab Emirates', '+971-4-555-6789', 'reservations@burjalarab.com', 1, 1, 1),
('The Savoy', 'Strand', 'London', 'United Kingdom', '+44-20-7555-1234', 'info@savoy.com', 1, 1, 1),
('Plaza Athénée', '25 Avenue Montaigne', 'Paris', 'France', '+33-1-5555-6789', 'reservations@plaza-athenee.com', 1, 1, 1),
('Mandarin Oriental', '5 Connaught Road', 'Hong Kong', 'China', '+852-2555-1234', 'mohkg@mandarinoriental.com', 1, 1, 1),
('Raffles Hotel', '1 Beach Road', 'Singapore', 'Singapore', '+65-6555-1234', 'singapore@raffles.com', 1, 1, 1),
('Waldorf Astoria', '301 Park Avenue', 'New York', 'United States', '+1-212-555-3456', 'reservations@waldorfastoria.com', 1, 1, 1),
('Four Seasons', '57 E 57th Street', 'New York', 'United States', '+1-212-555-4567', 'reservations.nyc@fourseasons.com', 1, 1, 1),
('Park Hyatt', '2 Rue de la Paix', 'Paris', 'France', '+33-1-5555-1234', 'paris.park@hyatt.com', 1, 1, 1),
('Marina Bay Sands', '10 Bayfront Avenue', 'Singapore', 'Singapore', '+65-6555-2345', 'reservations@marinabaysands.com', 1, 1, 1),
('The Peninsula', 'Salisbury Road', 'Hong Kong', 'China', '+852-2555-2345', 'phk@peninsula.com', 1, 1, 1),
('Claridges', 'Brook Street', 'London', 'United Kingdom', '+44-20-7555-2345', 'reservations@claridges.com', 1, 1, 1),
('Bellagio', '3600 Las Vegas Blvd S', 'Las Vegas', 'United States', '+1-702-555-1234', 'reservations@bellagio.com', 1, 1, 1),
('Atlantis', 'Paradise Island', 'Nassau', 'Bahamas', '+1-242-555-1234', 'reservations@atlantis.com', 1, 1, 1),
('The Venetian', '3355 Las Vegas Blvd S', 'Las Vegas', 'United States', '+1-702-555-2345', 'reservations@venetian.com', 1, 1, 1),
('InterContinental', '1 Hamilton Place', 'London', 'United Kingdom', '+44-20-7555-3456', 'london@intercontinental.com', 1, 1, 1),
('Hilton Tokyo', '6-6-2 Nishi-Shinjuku', 'Tokyo', 'Japan', '+81-3-5555-1234', 'tokyo@hilton.com', 1, 1, 1),
('Caesars Palace', '3570 Las Vegas Blvd S', 'Las Vegas', 'United States', '+1-702-555-3456', 'reservations@caesars.com', 1, 1, 1),
('Hotel Arts', '19-21 Marina', 'Barcelona', 'Spain', '+34-93-555-1234', 'arts@ritzcarlton.com', 1, 1, 1);

-- Create the Rooms table
DROP TABLE IF EXISTS Rooms;
GO
CREATE TABLE Rooms (
    -- Primary key identifier for the room
    Id INT IDENTITY(1,1) PRIMARY KEY,
    
    -- Foreign key reference to the Hotels table
    -- Identifies which hotel this room belongs to
    HotelId INT NOT NULL,
    
    -- Room number or identifier within the hotel
    -- Examples: "101", "A204", "Presidential Suite"
    RoomNumber NVARCHAR(20) NOT NULL,
    
    -- Type of room (foreign key to RoomTypes table)
    -- 1 = Standard, 2 = Deluxe, 3 = Suite, 4 = Executive, 5 = Penthouse, etc.
    RoomTypeId INT NOT NULL,
    
    -- Cost per night for staying in this room (in the hotel's local currency)
    PricePerNight DECIMAL(10, 2) NOT NULL,
    
    -- Indicates if the room is currently available for booking
    -- TRUE = available, FALSE = occupied or under maintenance
    IsAvailable BIT NOT NULL DEFAULT 1,
    
    -- Maximum number of guests allowed in the room
    MaxOccupancy INT NOT NULL,
    
    -- Status of the room record:
    -- 1 = Active (operational room)
    -- 2 = Inactive (temporarily not available)
    -- 3 = DeletedForEveryone (soft deleted from all users)
    -- 4 = Pending (awaiting approval/verification)
    -- 5 = Archived (historical record, no longer in service)
    -- 6 = Suspended (suspended due to policy violations)
    -- 7 = DeletedForMe (soft deleted for specific users)
    EntityStatusId INT NOT NULL DEFAULT(1),
    
    -- Timestamp when the room record was created
    CreateAt DATETIME NULL DEFAULT(GETUTCDATE()),
    
    -- User ID of the person who created the room record
    CreateBy INT NOT NULL,
    
    -- Timestamp when the room record was last updated
    UpdateAt DATETIME NULL,
    
    -- User ID of the person who last updated the room record
    UpdateBy INT NULL
);

-- Add indexes for better query performance
CREATE INDEX IX_Rooms_HotelId ON Rooms(HotelId);
CREATE INDEX IX_Rooms_RoomTypeId ON Rooms(RoomTypeId);
CREATE INDEX IX_Rooms_IsAvailable ON Rooms(IsAvailable);
CREATE INDEX IX_Rooms_EntityStatusId ON Rooms(EntityStatusId);

-- Add unique constraint to prevent duplicate room numbers within the same hotel
CREATE UNIQUE INDEX IX_Rooms_HotelId_RoomNumber ON Rooms(HotelId, RoomNumber) WHERE EntityStatusId = 1;

-- Insert 20 room records distributed across hotels with IDs 1-22
INSERT INTO Rooms (HotelId, RoomNumber, RoomTypeId, PricePerNight, IsAvailable, MaxOccupany, EntityStatusId, CreateBy, UpdateBy)
VALUES 
-- Grand Hyatt (ID 1)
(1, '101', 1, 299.99, 1, 2, 1, 1, 1),
(1, '102', 1, 299.99, 1, 2, 1, 1, 1),
(1, '201', 2, 459.99, 1, 3, 1, 1, 1),

-- Ritz-Carlton (ID 2)
(2, 'A101', 2, 699.99, 1, 2, 1, 1, 1),
(2, 'A201', 3, 1299.99, 0, 4, 1, 1, 1),

-- Burj Al Arab (ID 3)
(3, 'Suite 1', 5, 2999.99, 1, 4, 1, 1, 1),
(3, 'Suite 2', 5, 3499.99, 1, 6, 1, 1, 1),

-- The Savoy (ID 4)
(4, '301', 2, 549.99, 1, 2, 1, 1, 1),
(4, '302', 2, 549.99, 0, 2, 1, 1, 1),

-- Plaza Athénée (ID 5)
(5, '501', 3, 899.99, 1, 3, 1, 1, 1),

-- Mandarin Oriental (ID 6)
(6, '701', 1, 259.99, 1, 2, 1, 1, 1),
(6, '801', 4, 799.99, 1, 4, 1, 1, 1),

-- Four Seasons (ID 9)
(9, '1201', 3, 699.99, 1, 4, 1, 1, 1),
(9, '1202', 3, 699.99, 0, 4, 1, 1, 1),

-- Marina Bay Sands (ID 11)
(11, '2501', 4, 999.99, 1, 3, 1, 1, 1),

-- The Peninsula (ID 12)
(12, '601', 2, 449.99, 1, 2, 1, 1, 1),

-- Bellagio (ID 14)
(14, '1801', 3, 599.99, 1, 4, 1, 1, 1),

-- Atlantis (ID 15)
(15, 'Royal Suite', 5, 2499.99, 1, 6, 1, 1, 1),

-- Hilton Tokyo (ID 18)
(18, '901', 1, 239.99, 1, 2, 1, 1, 1),

-- Hotel Arts (ID 20)
(20, '1501', 2, 429.99, 1, 3, 1, 1, 1);


-- Create the RoomType table
DROP TABLE IF EXISTS RoomType;
GO
CREATE TABLE RoomType (
    -- Primary key ID
    Id INT PRIMARY KEY IDENTITY(1,1),
    -- Name of the room type
    Name NVARCHAR(100) NOT NULL,
    -- Detailed description of the room type
    Description NVARCHAR(500) NOT NULL,
    -- Entity status: 1=Active, 2=Inactive, 3=DeletedForEveryone, 4=Pending, 5=Archived, 6=Suspended, 7=DeletedForMe
    EntityStatusId INT NOT NULL DEFAULT 1,
    -- Creation tracking
    CreatedAt DATETIME2 NULL,
    CreatedBy INT NOT NULL,
    -- Update tracking
    UpdatedAt DATETIME2 NULL,
    UpdatedBy INT NOT NULL
);

-- Add index on EntityStatusId for faster filtering
CREATE INDEX IX_RoomType_EntityStatusId ON RoomTypes(EntityStatusId);

-- Insert statements for all room types from the enum
INSERT INTO RoomTypes (Name, Description, EntityStatusId, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES 
    ('Standard', 'Standard room with basic amenities', 1, GETDATE(), 1, GETDATE(), 1),
    ('Deluxe', 'Deluxe room with enhanced amenities and comfort', 1, GETDATE(), 1, GETDATE(), 1),
    ('Suite', 'Suite with separate living area and bedroom', 1, GETDATE(), 1, GETDATE(), 1),
    ('Executive', 'Executive room with business amenities and services', 1, GETDATE(), 1, GETDATE(), 1),
    ('Penthouse', 'Penthouse suite, typically on the top floor with premium amenities', 1, GETDATE(), 1, GETDATE(), 1),
    ('Family', 'Family room with additional space for families', 1, GETDATE(), 1, GETDATE(), 1),
    ('Accessible', 'Accessible room designed for guests with disabilities', 1, GETDATE(), 1, GETDATE(), 1),
    ('Single', 'Single room designed for one person', 1, GETDATE(), 1, GETDATE(), 1),
    ('Double', 'Double room with a queen or king-sized bed', 1, GETDATE(), 1, GETDATE(), 1),
    ('Twin', 'Twin room with two separate beds', 1, GETDATE(), 1, GETDATE(), 1),
    ('Presidential', 'Presidential suite, the most luxurious accommodation', 1, GETDATE(), 1, GETDATE(), 1),
    ('Villa', 'Villa or cottage separate from the main hotel building', 1, GETDATE(), 1, GETDATE(), 1);