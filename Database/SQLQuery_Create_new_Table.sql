/*
CREATE TABLE Train_Gsq.dbo.TR_Customer(
    Id           INT            NOT NULL     PRIMARY KEY,
	Email        VARCHAR (50)   NOT NULL,
	Name_        VARCHAR (10)   NOT NULL,
	Gender       TINYINT        NOT NULL,
	RegisterDate SMALLDATETIME      NULL,
	Spending     DECIMAL(10, 3) NOT NULL,
	Vip          TINYINT        NOT NULL,
	Active       BIT            NOT NULL,
);


INSERT INTO Train_Gsq.dbo.TR_Customer VALUES('88', '123@qq.com', 'Gsq', '0', '', '100.000', '3', '1');
*/

UPDATE Train_Gsq.dbo.TR_Customer SET RegisterDate = NULL WHERE Name_ = 'Gsq';

SELECT * FROM Train_Gsq.dbo.TR_Customer;
