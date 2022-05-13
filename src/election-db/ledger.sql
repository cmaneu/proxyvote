-- Create schema and table
CREATE SCHEMA [Election]
GO

CREATE TABLE [Election].[RegisteredList]
(
    [RegistrationId] INT NOT NULL PRIMARY KEY CLUSTERED,
    [FirstName] VARCHAR (50) NOT NULL,
    [LastName] VARCHAR (50) NOT NULL,
    [BirthDate] DATETIME NOT NULL,
    [BirthPlace] VARCHAR (50) NOT NULL,
    [RegistrationAddress] VARCHAR (MAX) NOT NULL,
    [RegistrationZipCode] VARCHAR (10) NOT NULL,
    [VoteOfficeId] VARCHAR (10) NOT NULL,
    [ProxyFirstName] VARCHAR (50) NULL,
    [ProxyLastName] VARCHAR (50) NULL,
    [ProxyBirthDate] DATETIME NULL
)
WITH 
(
 SYSTEM_VERSIONING = ON,
 LEDGER = ON
);
GO

-- Insert some data
INSERT INTO [Election].[RegisteredList]
( 
    [RegistrationId],
    [FirstName],
    [LastName] ,
    [BirthDate],
    [BirthPlace],
    [RegistrationAddress] ,
    [RegistrationZipCode] ,
    [VoteOfficeId]
)
VALUES
( 
  1,'Christopher','Maneu','1985-10-21','Toulouse', '102 Avenue des Minimes', '31000', '214'
)
GO

-- List transactions
SELECT * 
      ,[ledger_start_transaction_id]
      ,[ledger_end_transaction_id]
      ,[ledger_start_sequence_number]
      ,[ledger_end_sequence_number]
FROM [Election].[RegisteredList]


--- Try to replace a record
INSERT INTO [Election].[RegisteredList]
( 
    [RegistrationId],
    [FirstName],
    [LastName] ,
    [BirthDate],
    [BirthPlace],
    [RegistrationAddress] ,
    [RegistrationZipCode] ,
    [VoteOfficeId]
)
VALUES
( 
  2,'Sébastien','Dupont','1954-04-14','Paris', '45 Rue de la Convention', '75015', '198'
)
GO

DELETE FROM [Election].[RegisteredList] WHERE RegistrationId = 1


--- See the ledger content
SELECT * FROM Election.RegisteredList_Ledger
ORDER BY ledger_transaction_id
GO





--- Not important for the demo
EXECUTE sp_generate_database_ledger_digest