DELETE FROM Train_Gsq.dbo.TR_Customer WHERE Id > 1;
IDENTITY_INSERT Train_Gsq.dbo.TR_Customer ON
INSERT Train_Gsq.dbo.TR_Customer VALUES(1,'1@qq.com','gsq', 0,NULL,100.00, 1, 1)
IDENTITY_INSERT Train_Gsq.dbo.TR_Customer OFF
SELECT * FROM Train_Gsq.dbo.TR_Customer;
SELECT * FROM Train_Gsq.dbo.TR_User