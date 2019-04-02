
USE BrightOnionOrdering
GO

DELETE FROM OrderItems
DELETE FROM Orders
DELETE FROM Purchasers
GO

USE BrightOnionCatalog
GO

DELETE FROM dbo.IntegrationEventLog
DELETE FROM dbo.Room
GO

USE BrightOnionIdentity
GO

DELETE FROM dbo.IntegrationEventLogs
DELETE FROM dbo.Users
GO
