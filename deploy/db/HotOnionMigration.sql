
USE BrightOnionIdentity
GO

-- Migrate Users
BEGIN TRAN

INSERT INTO Users(Email, PasswordHash, NotificationEnabled)
SELECT Email, Passwd, ISNULL(EmailNotification, 0) FROM pizza_users 
WHERE Email NOT IN ( SELECT Email FROM Users )

ROLLBACK
--COMMIT
GO

-- Migrate Purchasers
BEGIN TRAN

INSERT INTO BrightOnionOrdering.dbo.Purchasers(Id, Email, NotificationEnabled)
SELECT Id, Email, NotificationEnabled FROM BrightOnionIdentity.dbo.Users
WHERE Id NOT IN ( SELECT Id FROM BrightOnionOrdering.dbo.Purchasers );

UPDATE p SET
	p.Email = u.Email,
	P.NotificationEnabled = u.NotificationEnabled
FROM BrightOnionOrdering.dbo.Purchasers p
INNER JOIN BrightOnionIdentity.dbo.Users u ON p.Id = u.Id;

ROLLBACK
--COMMIT
GO

-- Migrate Rooms
USE BrightOnionCatalog
GO

BEGIN TRAN

INSERT INTO Room (Name, SlicesPerPizza)
SELECT Name, 8 FROM pizza_rooms 
WHERE Name NOT IN ( SELECT Name FROM Room )

ROLLBACK
--COMMIT

-- TODO: Migrate Orders
USE BrightOnionOrdering
GO

BEGIN TRAN

--ALTER TABLE pizza_orders ADD RoomId BIGINT;
--ALTER TABLE pizza_orders ADD PurchaserId BIGINT;

UPDATE po SET
	po.RoomId = r.Id,
	po.PurchaserId = u.Id
FROM pizza_orders po
INNER JOIN BrightOnionCatalog.dbo.Room r ON r.Name = po.Room_Name
INNER JOIN BrightOnionIdentity.dbo.Users u ON u.Email = po.Who_Email;

INSERT INTO Orders(RoomId, Day, IsApproved, SlicesPerPizza, TotalPizzas, FreeSlicesToGrab)
SELECT po.RoomId, po.Day, 1, 8 /*, SUM(Quantity)*/
, IIF(SUM(po.Quantity) % 8 <> 0, SUM(po.Quantity) / 8 + 1, SUM(po.Quantity) / 8)
, IIF(SUM(po.Quantity) % 8 <> 0, SUM(po.Quantity) / 8 + 1, SUM(po.Quantity) / 8) * 8 - SUM(po.Quantity)
FROM pizza_orders po
LEFT JOIN Orders o ON o.Day = po.Day AND o.RoomId = po.RoomId
WHERE o.Id IS NULL
GROUP BY po.Day, po.RoomId;

INSERT INTO OrderItems(OrderId, PurchaserId, Quantity)
SELECT o.Id, PurchaserId, Quantity 
FROM pizza_orders po
INNER JOIN Orders o ON o.Day = po.Day AND o.RoomId = po.RoomId
;

ROLLBACK
--COMMIT
