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
    CreateAt DATETIME NOT NULL DEFAULT(GETUTCDATE()),
    
    -- User ID of the person who created the hotel record
    CreateBy INT NOT NULL,
    
    -- Timestamp when the hotel record was last updated
    UpdateAt DATETIME NULL,
    
    -- User ID of the person who last updated the hotel record
    UpdateBy INT  NULL
);

-- Add index for faster searching by name
CREATE INDEX IX_Hotels_Name ON Hotels(Name);

-- Add index for filtering by status
CREATE INDEX IX_Hotels_EntityStatusId ON Hotels(EntityStatusId);

-- Create UNIQUE index for preventing duplicate hotels at the same address
CREATE UNIQUE INDEX IX_Hotels_Name_Address ON Hotels(Name, Address) WHERE EntityStatusId = 1;

-- Insert 20 sample hotels
INSERT INTO Hotels (Name, Address, City, Country, PhoneNumber, Email, EntityStatusId, CreateBy, UpdateBy)
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