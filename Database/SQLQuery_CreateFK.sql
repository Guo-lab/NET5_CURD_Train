SELECT * FROM Train_Gsq.dbo.TR_User
SELECT * FROM Train_Gsq.dbo.TR_Customer


ALTER TABLE Train_Gsq.dbo.TR_Customer
ADD UserId INT NULL


ALTER TABLE Train_Gsq.dbo.TR_Customer
ADD FOREIGN KEY (UserId)
REFERENCES Train_Gsq.dbo.TR_User(Id)


/*
ALTER TABLE Train_Gsq.dbo.TR_Customer
ADD CONSTRAINT FK_TR_Customer_TR_User FOREIGN KEY (UserId)
REFERENCES Train_Gsq.dbo.TR_User(Id)


ALTER TABLE Train_Gsq.dbo.TR_Customer
DROP CONSTRAINT FK_TR_Customer_TR_User
*/

SELECT * FROM Train_Gsq.dbo.TR_Customer