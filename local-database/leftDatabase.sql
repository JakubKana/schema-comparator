USE [DatabaseComparatorTest1]
GO
/****** Object:  UserDefinedFunction [dbo].[StripWWWandCom]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[StripWWWandCom] (@input VARCHAR(250))
RETURNS VARCHAR(250)
AS BEGIN
    DECLARE @Work VARCHAR(250)

    SET @Work = @Input

    SET @Work = REPLACE(@Work, 'www.', '')
    SET @Work = REPLACE(@Work, '.com', '')

    RETURN @work
END

GO
/****** Object:  UserDefinedFunction [dbo].[StripWWWCom]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[StripWWWCom] (@input VARCHAR(250))
RETURNS INT
AS BEGIN
    DECLARE @Work INT

    SET @Work = @Input

    SET @Work = REPLACE(@Work, 'www.', '')
    SET @Work = REPLACE(@Work, '.com', '')

    RETURN 578
END

GO
/****** Object:  Table [dbo].[CarPerson]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarPerson](
	[ID] [int] NOT NULL,
	[CarId] [int] NULL,
	[PersonId] [int] NULL,
 CONSTRAINT [PK_CarPerson] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Cars]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cars](
	[ID] [int] NOT NULL,
	[ItemName] [varchar](50) NULL,
	[ItemPrice] [money] NULL,
	[ItemWeight] [numeric](18, 2) NOT NULL,
 CONSTRAINT [PK_Cars] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Companies]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies](
	[Company_ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Address] [varchar](200) NULL,
	[ICO] [varchar](50) NULL,
	[DIC] [varchar](50) NULL,
 CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED 
(
	[Company_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Company_contacts]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company_contacts](
	[Company_ID] [int] NOT NULL,
	[Contact_ID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Company_notes]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company_notes](
	[Company_ID] [int] NOT NULL,
	[Note_ID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Contact_directories]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contact_directories](
	[Contact_ID] [int] NOT NULL,
	[Directory_ID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Contact_types]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contact_types](
	[Contact_type_ID] [int] IDENTITY(1,1) NOT NULL,
	[Contact_type] [varchar](20) NOT NULL,
	[Format] [varchar](50) NULL,
	[Has_prefix] [nchar](1) NOT NULL,
	[Validation] [nchar](1) NOT NULL,
 CONSTRAINT [PK_Contact_types] PRIMARY KEY CLUSTERED 
(
	[Contact_type_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Contacts]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contacts](
	[Contact_ID] [int] IDENTITY(1,1) NOT NULL,
	[Contact_type_ID] [int] NOT NULL,
	[Contact] [varchar](100) NOT NULL,
	[Prefix] [varchar](5) NULL,
	[CreatedBy] [int] NULL,
 CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED 
(
	[Contact_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Directories]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Directories](
	[Directory_ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](30) NOT NULL,
	[Directory_Order] [int] NOT NULL,
	[ShowOnPhone] [bit] NOT NULL,
	[ShowIdentification] [bit] NOT NULL,
	[NonPublicDir] [bit] NOT NULL,
 CONSTRAINT [PK_Directories] PRIMARY KEY CLUSTERED 
(
	[Directory_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Notes]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notes](
	[Note_ID] [int] IDENTITY(1,1) NOT NULL,
	[Note] [varchar](250) NULL,
 CONSTRAINT [PK_Notes] PRIMARY KEY CLUSTERED 
(
	[Note_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Person]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Person](
	[ID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Surname] [nchar](10) NULL,
	[Sex] [bit] NOT NULL,
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Person_contacts]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Person_contacts](
	[Person_ID] [int] NOT NULL,
	[Contact_ID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Person_notes]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Person_notes](
	[Person_ID] [int] NOT NULL,
	[Note_ID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Persons]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Persons](
	[Person_ID] [int] IDENTITY(1,1) NOT NULL,
	[Company_ID] [int] NOT NULL,
	[Name] [varchar](50) NULL,
	[Surname] [varchar](50) NULL,
	[title_befor] [varchar](50) NULL,
	[title_behind] [varchar](50) NULL,
	[POSITION] [varchar](100) NULL,
	[Department] [varchar](50) NULL,
	[CreatedBy] [int] NULL,
 CONSTRAINT [PK_Persons] PRIMARY KEY CLUSTERED 
(
	[Person_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Product]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ID] [int] NULL,
	[Name] [nchar](10) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[User_ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](20) NOT NULL,
	[Password] [varchar](200) NOT NULL,
	[ROLE] [int] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[User_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  UserDefinedFunction [dbo].[INFOSCH]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[INFOSCH] (@storeid int)  
RETURNS TABLE  
AS  
RETURN   
(  
    SELECT * FROM INFORMATION_SCHEMA.TABLES
);  

GO
/****** Object:  UserDefinedFunction [dbo].[LUDACRIS]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[LUDACRIS] (@storeid int)  
RETURNS TABLE  
AS  
RETURN   
(  
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS
);  
GO
/****** Object:  View [dbo].[Companies_contact_view]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Companies_contact_view] AS

SELECT     Companies.*, Contacts.Contact
FROM         Companies LEFT OUTER JOIN
                      Company_contacts ON Companies.Company_ID = Company_contacts.Company_ID LEFT OUTER JOIN
                      Contacts ON Company_contacts.Contact_ID = Contacts.Contact_ID LEFT OUTER JOIN
                      Contact_types ON Contacts.Contact_type_ID = Contact_types.Contact_type_ID


GO
/****** Object:  View [dbo].[Companies_contacts_notes_view]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Companies_contacts_notes_view]
AS
SELECT        dbo.Companies.Company_ID, dbo.Companies.Name, dbo.Companies.Address, dbo.Companies.ICO, dbo.Companies.DIC, dbo.Contacts.Contact_ID, dbo.Contacts.Contact_type_ID, dbo.Contacts.Contact, 
                         dbo.Contact_types.Contact_type, dbo.Notes.Note, dbo.Contacts.Prefix, dbo.Contact_types.Validation, dbo.Contact_types.Has_prefix
FROM            dbo.Companies LEFT OUTER JOIN
                         dbo.Company_contacts ON dbo.Companies.Company_ID = dbo.Company_contacts.Company_ID LEFT OUTER JOIN
                         dbo.Company_notes ON dbo.Companies.Company_ID = dbo.Company_notes.Company_ID LEFT OUTER JOIN
                         dbo.Contacts ON dbo.Company_contacts.Contact_ID = dbo.Contacts.Contact_ID LEFT OUTER JOIN
                         dbo.Contact_types ON dbo.Contacts.Contact_type_ID = dbo.Contact_types.Contact_type_ID LEFT OUTER JOIN
                         dbo.Notes ON dbo.Company_notes.Note_ID = dbo.Notes.Note_ID


GO
/****** Object:  View [dbo].[Companies_contacts_notes_view_restricted]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Companies_contacts_notes_view_restricted]
AS
SELECT        dbo.Companies.Company_ID, dbo.Companies.Name, dbo.Companies.Address, dbo.Companies.ICO, dbo.Companies.DIC, dbo.Contacts.Contact_ID, dbo.Contacts.Contact_type_ID, dbo.Contacts.Contact, 
                         dbo.Contact_types.Contact_type, dbo.Notes.Note, dbo.Contacts.Prefix, dbo.Contact_types.Validation, dbo.Contact_types.Has_prefix
FROM            dbo.Companies LEFT OUTER JOIN
                         dbo.Company_contacts ON dbo.Companies.Company_ID = dbo.Company_contacts.Company_ID LEFT OUTER JOIN
                         dbo.Company_notes ON dbo.Companies.Company_ID = dbo.Company_notes.Company_ID LEFT OUTER JOIN
                         dbo.Contacts ON dbo.Company_contacts.Contact_ID = dbo.Contacts.Contact_ID LEFT OUTER JOIN
                         dbo.Contact_types ON dbo.Contacts.Contact_type_ID = dbo.Contact_types.Contact_type_ID LEFT OUTER JOIN
                         dbo.Notes ON dbo.Company_notes.Note_ID = dbo.Notes.Note_ID
WHERE        (dbo.Contacts.Contact_ID IN
                             (SELECT        dbo.Contact_directories.Contact_ID
                               FROM            dbo.Contact_directories INNER JOIN
                                                         dbo.Directories ON dbo.Contact_directories.Directory_ID = dbo.Directories.Directory_ID
                               WHERE        (dbo.Directories.NonPublicDir = 0)))


GO
/****** Object:  View [dbo].[Company_notes_view]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Company_notes_view]
AS
SELECT     dbo.Companies.Company_ID, dbo.Companies.Name, dbo.Companies.Address, dbo.Companies.ICO, dbo.Companies.DIC, dbo.Notes.Note
FROM         dbo.Companies LEFT OUTER JOIN
                      dbo.Company_notes ON dbo.Companies.Company_ID = dbo.Company_notes.Company_ID LEFT OUTER JOIN
                      dbo.Notes ON dbo.Company_notes.Note_ID = dbo.Notes.Note_ID


GO
/****** Object:  View [dbo].[Company_notes_view_restricted]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Company_notes_view_restricted]
AS
SELECT        dbo.Companies.Company_ID, dbo.Companies.Name, dbo.Companies.Address, dbo.Companies.ICO, dbo.Companies.DIC, dbo.Notes.Note
FROM            dbo.Companies LEFT OUTER JOIN
                         dbo.Company_notes ON dbo.Companies.Company_ID = dbo.Company_notes.Company_ID LEFT OUTER JOIN
                         dbo.Notes ON dbo.Company_notes.Note_ID = dbo.Notes.Note_ID
WHERE        (dbo.Companies.Company_ID IN
                             (SELECT        dbo.Company_contacts.Company_ID
                               FROM            dbo.Company_contacts LEFT OUTER JOIN
                                                         dbo.Contacts ON dbo.Company_contacts.Contact_ID = dbo.Contacts.Contact_ID LEFT OUTER JOIN
                                                         dbo.Contact_directories ON dbo.Contact_directories.Contact_ID = dbo.Contacts.Contact_ID LEFT OUTER JOIN
                                                         dbo.Directories ON dbo.Contact_directories.Directory_ID = dbo.Directories.Directory_ID
                               WHERE        (dbo.Directories.NonPublicDir = 0)))


GO
/****** Object:  View [dbo].[Identification_contacts_view]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Identification_contacts_view]
AS
SELECT        p.Name, p.Surname, REPLACE(c.Prefix + c.Contact, '+', '00') AS Contact, comppers.Name AS Person_Company_Name, comp.Name AS Company_Name, p.title_befor, p.title_behind, p.POSITION, p.Department, 
                         dir.ShowIdentification AS Identification
FROM            dbo.Contacts AS c LEFT OUTER JOIN
                         dbo.Person_contacts AS pc ON c.Contact_ID = pc.Contact_ID LEFT OUTER JOIN
                         dbo.Company_contacts AS cc ON c.Contact_ID = cc.Contact_ID LEFT OUTER JOIN
                         dbo.Persons AS p ON p.Person_ID = pc.Person_ID LEFT OUTER JOIN
                         dbo.Companies AS comp ON cc.Company_ID = comp.Company_ID LEFT OUTER JOIN
                         dbo.Companies AS comppers ON p.Company_ID = comppers.Company_ID INNER JOIN
                         dbo.Contact_types AS ct ON c.Contact_type_ID = ct.Contact_type_ID INNER JOIN
                         dbo.Contact_directories AS con_dir ON c.Contact_ID = con_dir.Contact_ID INNER JOIN
                         dbo.Directories AS dir ON con_dir.Directory_ID = dir.Directory_ID
WHERE        (ct.Validation = 'T')


GO
/****** Object:  View [dbo].[Persons_companies_contacts_notes_view]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Persons_companies_contacts_notes_view]
AS
SELECT        dbo.Companies.Name AS Company_Name, dbo.Persons.Person_ID, dbo.Persons.Name, dbo.Persons.Surname, dbo.Persons.title_befor, dbo.Persons.POSITION, dbo.Notes.Note, dbo.Contact_types.Contact_type, 
                         dbo.Contacts.Contact, dbo.Contacts.Contact_ID, dbo.Contacts.Contact_type_ID, dbo.Companies.Company_ID, dbo.Notes.Note_ID, dbo.Contacts.Prefix, dbo.Contact_types.Validation, dbo.Contact_types.Has_prefix, 
                         dbo.Persons.Department, dbo.Persons.title_behind
FROM            dbo.Companies INNER JOIN
                         dbo.Persons ON dbo.Companies.Company_ID = dbo.Persons.Company_ID LEFT OUTER JOIN
                         dbo.Person_notes ON dbo.Persons.Person_ID = dbo.Person_notes.Person_ID LEFT OUTER JOIN
                         dbo.Person_contacts ON dbo.Persons.Person_ID = dbo.Person_contacts.Person_ID LEFT OUTER JOIN
                         dbo.Contacts ON dbo.Person_contacts.Contact_ID = dbo.Contacts.Contact_ID LEFT OUTER JOIN
                         dbo.Contact_types ON dbo.Contacts.Contact_type_ID = dbo.Contact_types.Contact_type_ID LEFT OUTER JOIN
                         dbo.Notes ON dbo.Person_notes.Note_ID = dbo.Notes.Note_ID


GO
/****** Object:  View [dbo].[Persons_companies_contacts_notes_view_restricted]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Persons_companies_contacts_notes_view_restricted]
AS
SELECT        dbo.Companies.Name AS Company_Name, dbo.Persons.Person_ID, dbo.Persons.Name, dbo.Persons.Surname, dbo.Persons.title_befor, dbo.Persons.POSITION, dbo.Notes.Note, dbo.Contact_types.Contact_type, 
                         dbo.Contacts.Contact, dbo.Contacts.Contact_ID, dbo.Contacts.Contact_type_ID, dbo.Companies.Company_ID, dbo.Notes.Note_ID, dbo.Contacts.Prefix, dbo.Contact_types.Validation, dbo.Contact_types.Has_prefix, 
                         dbo.Persons.Department, dbo.Persons.title_behind
FROM            dbo.Companies LEFT OUTER JOIN
                         dbo.Persons ON dbo.Companies.Company_ID = dbo.Persons.Company_ID LEFT OUTER JOIN
                         dbo.Person_notes ON dbo.Persons.Person_ID = dbo.Person_notes.Person_ID LEFT OUTER JOIN
                         dbo.Person_contacts ON dbo.Persons.Person_ID = dbo.Person_contacts.Person_ID LEFT OUTER JOIN
                         dbo.Contacts ON dbo.Person_contacts.Contact_ID = dbo.Contacts.Contact_ID LEFT OUTER JOIN
                         dbo.Contact_types ON dbo.Contacts.Contact_type_ID = dbo.Contact_types.Contact_type_ID LEFT OUTER JOIN
                         dbo.Notes ON dbo.Person_notes.Note_ID = dbo.Notes.Note_ID
WHERE        (dbo.Contacts.Contact_ID IN
                             (SELECT        Contacts_1.Contact_ID
                               FROM            dbo.Person_contacts AS Person_contacts_1 INNER JOIN
                                                         dbo.Contacts AS Contacts_1 ON Person_contacts_1.Contact_ID = Contacts_1.Contact_ID INNER JOIN
                                                         dbo.Contact_directories ON Contacts_1.Contact_ID = dbo.Contact_directories.Contact_ID INNER JOIN
                                                         dbo.Directories ON dbo.Contact_directories.Directory_ID = dbo.Directories.Directory_ID
                               WHERE        (dbo.Directories.NonPublicDir = 0)))


GO
/****** Object:  View [dbo].[Persons_searching_view]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Persons_searching_view]
AS
SELECT        dbo.Companies.Name AS Company_Name, dbo.Persons.Person_ID, dbo.Persons.Name, dbo.Persons.Surname, dbo.Persons.POSITION, dbo.Notes.Note, dbo.Companies.Company_ID, dbo.Notes.Note_ID, 
                         dbo.Persons.Department
FROM            dbo.Companies INNER JOIN
                         dbo.Persons ON dbo.Companies.Company_ID = dbo.Persons.Company_ID LEFT OUTER JOIN
                         dbo.Person_notes ON dbo.Persons.Person_ID = dbo.Person_notes.Person_ID LEFT OUTER JOIN
                         dbo.Notes ON dbo.Person_notes.Note_ID = dbo.Notes.Note_ID


GO
/****** Object:  View [dbo].[Select_companies_notes_view]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Select_companies_notes_view]
AS
SELECT     dbo.Companies.Company_ID, dbo.Companies.Name, dbo.Companies.Address, dbo.Companies.ICO, dbo.Companies.DIC, dbo.Notes.Note_ID, 
                      dbo.Notes.Note
FROM         dbo.Companies LEFT OUTER JOIN
                      dbo.Company_notes ON dbo.Companies.Company_ID = dbo.Company_notes.Company_ID LEFT OUTER JOIN
                      dbo.Notes ON dbo.Company_notes.Note_ID = dbo.Notes.Note_ID


GO
/****** Object:  View [dbo].[Select_person_contacts_view_restricted]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Select_person_contacts_view_restricted]
AS
SELECT        Cont.Contact_ID, Cont.Contact_type_ID, Cont.Contact, Cont.Prefix, Cont.CreatedBy, PerCont.Person_ID, PerCont.Contact_ID AS Expr1, ConType.Contact_type_ID AS Expr2, ConType.Contact_type, ConType.Format, 
                         ConType.Has_prefix, ConType.Validation, ContDir.Contact_ID AS Expr3, ContDir.Directory_ID, Dir.Directory_ID AS Expr4, Dir.Name, Dir.Directory_Order, Dir.ShowOnPhone, Dir.ShowIdentification, 
                         Dir.NonPublicDir
FROM            dbo.Contacts AS Cont INNER JOIN
                         dbo.Person_contacts AS PerCont ON Cont.Contact_ID = PerCont.Contact_ID LEFT OUTER JOIN
                         dbo.Contact_types AS ConType ON ConType.Contact_type_ID = Cont.Contact_type_ID INNER JOIN
                         dbo.Contact_directories AS ContDir ON Cont.Contact_ID = ContDir.Contact_ID LEFT OUTER JOIN
                         dbo.Directories AS Dir ON ContDir.Directory_ID = Dir.Directory_ID
WHERE        (Dir.NonPublicDir = 0)


GO
/****** Object:  View [dbo].[Select_persons_notes_view]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Select_persons_notes_view]
AS
SELECT     dbo.Persons.Person_ID, dbo.Persons.Name, dbo.Persons.Surname, dbo.Persons.title_befor, dbo.Persons.POSITION, dbo.Notes.Note, 
                      dbo.Notes.Note_ID
FROM         dbo.Persons LEFT OUTER JOIN
                      dbo.Person_notes ON dbo.Persons.Person_ID = dbo.Person_notes.Person_ID LEFT OUTER JOIN
                      dbo.Notes ON dbo.Person_notes.Note_ID = dbo.Notes.Note_ID


GO
/****** Object:  View [dbo].[View_Cars]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_Cars]
AS
SELECT        ItemName, ItemPrice, ItemWeight
FROM            dbo.Cars

GO
ALTER TABLE [dbo].[Directories] ADD  DEFAULT ((1)) FOR [ShowOnPhone]
GO
ALTER TABLE [dbo].[Directories] ADD  DEFAULT ((1)) FOR [ShowIdentification]
GO
ALTER TABLE [dbo].[Directories] ADD  DEFAULT ((1)) FOR [NonPublicDir]
GO
ALTER TABLE [dbo].[CarPerson]  WITH CHECK ADD  CONSTRAINT [FK_CarPerson_Cars] FOREIGN KEY([CarId])
REFERENCES [dbo].[Cars] ([ID])
GO
ALTER TABLE [dbo].[CarPerson] CHECK CONSTRAINT [FK_CarPerson_Cars]
GO
ALTER TABLE [dbo].[CarPerson]  WITH CHECK ADD  CONSTRAINT [FK_CarPerson_Person] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([ID])
GO
ALTER TABLE [dbo].[CarPerson] CHECK CONSTRAINT [FK_CarPerson_Person]
GO
ALTER TABLE [dbo].[Company_contacts]  WITH CHECK ADD  CONSTRAINT [FK_Company_contacts_Companies] FOREIGN KEY([Company_ID])
REFERENCES [dbo].[Companies] ([Company_ID])
GO
ALTER TABLE [dbo].[Company_contacts] CHECK CONSTRAINT [FK_Company_contacts_Companies]
GO
ALTER TABLE [dbo].[Company_contacts]  WITH CHECK ADD  CONSTRAINT [FK_Company_contacts_Contacts] FOREIGN KEY([Contact_ID])
REFERENCES [dbo].[Contacts] ([Contact_ID])
GO
ALTER TABLE [dbo].[Company_contacts] CHECK CONSTRAINT [FK_Company_contacts_Contacts]
GO
ALTER TABLE [dbo].[Company_notes]  WITH CHECK ADD  CONSTRAINT [FK_Company_notes_Companies] FOREIGN KEY([Company_ID])
REFERENCES [dbo].[Companies] ([Company_ID])
GO
ALTER TABLE [dbo].[Company_notes] CHECK CONSTRAINT [FK_Company_notes_Companies]
GO
ALTER TABLE [dbo].[Company_notes]  WITH CHECK ADD  CONSTRAINT [FK_Company_notes_Notes] FOREIGN KEY([Note_ID])
REFERENCES [dbo].[Notes] ([Note_ID])
GO
ALTER TABLE [dbo].[Company_notes] CHECK CONSTRAINT [FK_Company_notes_Notes]
GO
ALTER TABLE [dbo].[Contact_directories]  WITH CHECK ADD  CONSTRAINT [FK_Contact_directories_Contacts] FOREIGN KEY([Contact_ID])
REFERENCES [dbo].[Contacts] ([Contact_ID])
GO
ALTER TABLE [dbo].[Contact_directories] CHECK CONSTRAINT [FK_Contact_directories_Contacts]
GO
ALTER TABLE [dbo].[Contact_directories]  WITH CHECK ADD  CONSTRAINT [FK_Contact_directories_Directories] FOREIGN KEY([Directory_ID])
REFERENCES [dbo].[Directories] ([Directory_ID])
GO
ALTER TABLE [dbo].[Contact_directories] CHECK CONSTRAINT [FK_Contact_directories_Directories]
GO
ALTER TABLE [dbo].[Contacts]  WITH CHECK ADD  CONSTRAINT [FK_Contacts_Contact_types] FOREIGN KEY([Contact_type_ID])
REFERENCES [dbo].[Contact_types] ([Contact_type_ID])
GO
ALTER TABLE [dbo].[Contacts] CHECK CONSTRAINT [FK_Contacts_Contact_types]
GO
ALTER TABLE [dbo].[Person_contacts]  WITH CHECK ADD  CONSTRAINT [FK_Person_contacts_Contacts] FOREIGN KEY([Contact_ID])
REFERENCES [dbo].[Contacts] ([Contact_ID])
GO
ALTER TABLE [dbo].[Person_contacts] CHECK CONSTRAINT [FK_Person_contacts_Contacts]
GO
ALTER TABLE [dbo].[Person_contacts]  WITH CHECK ADD  CONSTRAINT [FK_Person_contacts_Persons] FOREIGN KEY([Person_ID])
REFERENCES [dbo].[Persons] ([Person_ID])
GO
ALTER TABLE [dbo].[Person_contacts] CHECK CONSTRAINT [FK_Person_contacts_Persons]
GO
ALTER TABLE [dbo].[Person_notes]  WITH CHECK ADD  CONSTRAINT [FK_Person_notes_Notes] FOREIGN KEY([Note_ID])
REFERENCES [dbo].[Notes] ([Note_ID])
GO
ALTER TABLE [dbo].[Person_notes] CHECK CONSTRAINT [FK_Person_notes_Notes]
GO
ALTER TABLE [dbo].[Person_notes]  WITH CHECK ADD  CONSTRAINT [FK_Person_notes_Persons] FOREIGN KEY([Person_ID])
REFERENCES [dbo].[Persons] ([Person_ID])
GO
ALTER TABLE [dbo].[Person_notes] CHECK CONSTRAINT [FK_Person_notes_Persons]
GO
ALTER TABLE [dbo].[Persons]  WITH CHECK ADD  CONSTRAINT [FK_Persons_Companies] FOREIGN KEY([Company_ID])
REFERENCES [dbo].[Companies] ([Company_ID])
GO
ALTER TABLE [dbo].[Persons] CHECK CONSTRAINT [FK_Persons_Companies]
GO
ALTER TABLE [dbo].[Cars]  WITH CHECK ADD  CONSTRAINT [CK_Cars] CHECK  (([ID]<(100)))
GO
ALTER TABLE [dbo].[Cars] CHECK CONSTRAINT [CK_Cars]
GO
ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [CK_Person] CHECK  (([ID]>=(0) AND [ID]<=(10000)))
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [CK_Person]
GO
/****** Object:  StoredProcedure [dbo].[Companies_contact_by_Company_ID]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[Companies_contact_by_Company_ID]



@Company_ID int



as



select * from Companies_contact_view where Company_ID = @Company_ID



GO
/****** Object:  StoredProcedure [dbo].[Delete_company_with_its_persons]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Delete_company_with_its_persons]



@Company_ID int



AS



BEGIN

-----mazani osob a jejich kontaktu a poznamek---------------

CREATE TABLE #Person_IDs(Person_ID int)

INSERT #Person_IDs (Person_ID)

SELECT Person_ID FROM Persons 

WHERE Company_ID = @Company_ID



---------mazani kontaktu osob------------------------------

CREATE TABLE #Contact_IDs(Contact_ID int)

INSERT #Contact_IDs(Contact_ID)

SELECT Contact_ID FROM Person_contacts 

WHERE Person_ID IN (SELECT Person_ID FROM #Person_IDs)



DELETE FROM Person_contacts

WHERE Contact_ID IN (SELECT Contact_ID FROM #Contact_IDs)



DELETE FROM Contact_directories

WHERE Contact_ID IN (SELECT Contact_ID FROM #Contact_IDs)



DELETE FROM Contacts 

WHERE Contact_ID IN (SELECT Contact_ID FROM #Contact_IDs)



DROP TABLE #Contact_IDs



---------mazani poznamek osob------------------------------

CREATE TABLE #Note_IDs(Note_ID int)

INSERT #Note_IDs (Note_ID)

SELECT Note_ID FROM Person_notes

WHERE Person_ID IN (SELECT Person_ID FROM #Person_IDs)



DELETE FROM Person_notes

WHERE Note_ID IN (SELECT Note_ID FROM #Note_IDs)



DELETE FROM Notes

WHERE Note_ID IN (SELECT Note_ID FROM #Note_IDs)



DROP TABLE #Note_IDs

DROP TABLE #Person_IDs



---------mazani osob---------------------------------------

DELETE FROM Persons

WHERE Company_ID = @Company_ID





-----------------------------------------------------------

--------mazani poznamek a kontaktu firmy a firmy-----------



CREATE TABLE #Company_contacts (Contact_ID int)

INSERT #Company_contacts (Contact_ID)

SELECT Contact_ID FROM Company_contacts

WHERE Company_ID = @Company_ID



--------smazani kontaktu-----------------------------------

DELETE FROM Company_contacts

WHERE Company_ID = @Company_ID



DELETE FROM Contact_directories

WHERE Contact_ID IN (SELECT Contact_ID FROM #Company_contacts)



DELETE FROM Contacts

WHERE Contact_ID IN (SELECT Contact_ID FROM #Company_contacts)



DROP TABLE #Company_contacts



--------smazani poznamek-----------------------------------

declare @Note_ID int

SET @Note_ID = (SELECT Note_ID FROM Company_notes WHERE Company_ID = @Company_ID)



DELETE FROM Company_notes

WHERE Note_ID = @Note_ID



DELETE FROM Notes

WHERE Note_ID = @Note_ID



-------smazani firmy---------------------------------------

DELETE FROM Companies

WHERE Company_ID = @Company_ID



END



GO
/****** Object:  StoredProcedure [dbo].[Delete_companyContacts]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Delete_companyContacts]



@Contact_ID int



AS



DELETE FROM Company_contacts

WHERE Contact_ID = @Contact_ID



DELETE FROM Contact_directories

WHERE Contact_ID = @Contact_ID



DELETE FROM Contacts

WHERE Contact_ID = @Contact_ID



GO
/****** Object:  StoredProcedure [dbo].[Delete_contacts2]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Delete_contacts2]



@Contact_ID int



AS



BEGIN



DELETE FROM Person_contacts

WHERE Contact_ID = @Contact_ID



DELETE FROM Contact_directories

WHERE Contact_ID = @Contact_ID



DELETE FROM Contacts

WHERE Contact_ID = @Contact_ID



END



GO
/****** Object:  StoredProcedure [dbo].[Delete_directory]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[Delete_directory]



@Contact_ID int



AS

BEGIN

DELETE FROM Contact_directories

WHERE ((Contact_ID = @Contact_ID))

END



GO
/****** Object:  StoredProcedure [dbo].[Delete_person_note_contact]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Delete_person_note_contact]



@Person_ID int



AS



BEGIN



CREATE TABLE #Person_contacts (Contact_ID int)

INSERT #Person_contacts (Contact_ID)

SELECT Contact_ID 

FROM Person_contacts 

WHERE Person_ID = @Person_ID



--------smaze kontakty k dane osobe------------------------

DELETE FROM Person_contacts

WHERE Person_ID = @Person_ID



DELETE FROM Contact_directories

WHERE Contact_ID IN (SELECT Contact_ID FROM #Person_contacts)



DELETE FROM Contacts

WHERE Contact_ID IN (SELECT Contact_ID FROM #Person_contacts)



DROP TABLE #Person_contacts



-----------smazani poznamky k osobe------------------------

declare @Note_ID int

SET @Note_ID = (SELECT Note_ID FROM Person_notes WHERE Person_ID = @Person_ID)



DELETE FROM Person_notes

WHERE Note_ID = @Note_ID



DELETE FROM Notes

WHERE Note_ID = @Note_ID



-----------smazani osoby-----------------------------------

DELETE FROM Persons

WHERE Person_ID = @Person_ID



END



GO
/****** Object:  StoredProcedure [dbo].[Get_complete_directory_list]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_complete_directory_list]

@Name VARCHAR(50),
@Surname VARCHAR(50),
@Department VARCHAR(50),
@Company VARCHAR(50)

AS

IF (@Name = '%' AND @Surname = '%' AND @Department = '%')
BEGIN
	SELECT     dbo.Companies.Name as Nam, dbo.Companies.Name as Dep, dbo.Contacts.Prefix, dbo.Contacts.Contact, null as PersonName, null as PersonSurname, null as Department, Companies.Name as CompanyName, Contact_type as ContactType
	FROM         dbo.Companies INNER JOIN
						  dbo.Company_contacts ON dbo.Companies.Company_ID = dbo.Company_contacts.Company_ID INNER JOIN
						  dbo.Contacts ON dbo.Company_contacts.Contact_ID = dbo.Contacts.Contact_ID INNER JOIN
						  dbo.Contact_types ON dbo.Contacts.Contact_type_ID = dbo.Contact_types.Contact_type_ID INNER JOIN
					      dbo.Contact_directories ON dbo.Contacts.Contact_ID = dbo.Contact_directories.Contact_ID INNER JOIN
					      dbo.Directories ON dbo.Contact_directories.Directory_ID = dbo.Directories.Directory_ID 
	WHERE				dbo.Directories.ShowOnPhone = 1	AND 
						(Contact_types.Validation = 'T' AND 
						dbo.Companies.Name LIKE @Company + '%')
	UNION


	SELECT     rtrim(ltrim(dbo.Persons.Surname + ' ' + dbo.Persons.Name)) as Nam, ltrim(dbo.Persons.Department + ' ' + dbo.Companies.Name) as Dep, dbo.Contacts.Prefix, dbo.Contacts.Contact, Persons.Name as PersonName, Persons.Surname as PersonSurname, Persons.Department as Department, Companies.Name as CompanyName, Contact_type as ContactType
	FROM         dbo.Person_contacts INNER JOIN
					  dbo.Persons ON dbo.Person_contacts.Person_ID = dbo.Persons.Person_ID INNER JOIN
					  dbo.Contacts ON dbo.Person_contacts.Contact_ID = dbo.Contacts.Contact_ID INNER JOIN
					  dbo.Companies ON dbo.Persons.Company_ID = dbo.Companies.Company_ID INNER JOIN
					  dbo.Contact_types ON dbo.Contacts.Contact_type_ID = dbo.Contact_types.Contact_type_ID INNER JOIN
					  dbo.Contact_directories ON dbo.Contacts.Contact_ID = dbo.Contact_directories.Contact_ID INNER JOIN
					  dbo.Directories ON dbo.Contact_directories.Directory_ID = dbo.Directories.Directory_ID 
	WHERE				dbo.Directories.ShowOnPhone = 1	AND 
					  ( (Persons.Name LIKE @Name + '%') AND
						(Persons.Surname LIKE @Surname + '%') AND
						(Persons.Department LIKE @Department + '%') AND
						(Companies.Name LIKE @Company + '%')
					  ) AND
					  (
						(Contact_types.Validation = 'T')
					  )
	ORDER BY Nam, Dep
END

ELSE

BEGIN
	SELECT     rtrim(ltrim(dbo.Persons.Surname + ' ' + dbo.Persons.Name)) as Nam, ltrim(dbo.Persons.Department + ' ' + dbo.Companies.Name) as Dep, dbo.Contacts.Prefix, dbo.Contacts.Contact, Persons.Name as PersonName, Persons.Surname as PersonSurname, Persons.Department as Department, Companies.Name as CompanyName, Contact_type as ContactType
	FROM         dbo.Person_contacts INNER JOIN
						  dbo.Persons ON dbo.Person_contacts.Person_ID = dbo.Persons.Person_ID INNER JOIN
						  dbo.Contacts ON dbo.Person_contacts.Contact_ID = dbo.Contacts.Contact_ID INNER JOIN
						  dbo.Companies ON dbo.Persons.Company_ID = dbo.Companies.Company_ID INNER JOIN
						  dbo.Contact_types ON dbo.Contacts.Contact_type_ID = dbo.Contact_types.Contact_type_ID INNER JOIN
						  dbo.Contact_directories ON dbo.Contacts.Contact_ID = dbo.Contact_directories.Contact_ID INNER JOIN
						  dbo.Directories ON dbo.Contact_directories.Directory_ID = dbo.Directories.Directory_ID 
	WHERE				dbo.Directories.ShowOnPhone = 1	AND 
						  ( (Persons.Name LIKE @Name + '%') AND
							(Persons.Surname LIKE @Surname + '%') AND
							(Persons.Department LIKE @Department + '%') AND
							(Companies.Name LIKE @Company + '%')
						  ) AND
						  (
							(Contact_types.Validation = 'T')
						  )
	ORDER BY Nam, Dep

END


GO
/****** Object:  StoredProcedure [dbo].[Get_Contact_ID_by_Company_ID]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_Contact_ID_by_Company_ID]



@Contact varchar(100),

@Company_ID int



AS



BEGIN

SELECT dbo.Contacts.Contact_ID

FROM dbo.Contacts INNER JOIN

dbo.Company_contacts ON dbo.Contacts.Contact_ID = dbo.Company_contacts.Contact_ID INNER JOIN

dbo.Companies ON dbo.Company_contacts.Company_ID = dbo.Companies.Company_ID

WHERE (dbo.Contacts.Contact = @Contact) AND (dbo.Company_contacts.Company_ID = @Company_ID)

END



GO
/****** Object:  StoredProcedure [dbo].[Get_Contact_ID_by_Person_ID]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_Contact_ID_by_Person_ID]



@Contact varchar(100),

@Person_ID int



AS



BEGIN

SELECT dbo.Contacts.Contact_ID

FROM dbo.Contacts INNER JOIN

dbo.Person_contacts ON dbo.Contacts.Contact_ID = dbo.Person_contacts.Contact_ID INNER JOIN

dbo.Persons ON dbo.Person_contacts.Person_ID = dbo.Persons.Person_ID

WHERE (dbo.Contacts.Contact = @Contact) AND (dbo.Person_contacts.Person_ID = @Person_ID)

END



GO
/****** Object:  StoredProcedure [dbo].[Get_directory_ID]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[Get_directory_ID]



@Contact_ID int



AS

BEGIN

SELECT dbo.Directories.Directory_ID

FROM dbo.Contacts INNER JOIN

dbo.Contact_directories ON dbo.Contacts.Contact_ID = dbo.Contact_directories.Contact_ID INNER JOIN

dbo.Directories ON dbo.Contact_directories.Directory_ID = dbo.Directories.Directory_ID

WHERE (dbo.Contacts.Contact_ID = @Contact_ID)

END



GO
/****** Object:  StoredProcedure [dbo].[Get_directory_list]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_directory_list]

@Name VARCHAR(50),
@Surname VARCHAR(50),
@Department VARCHAR(50),
@DirId int,
@Company VARCHAR(50) = ''

AS

SELECT     dbo.Companies.Name as Nam, dbo.Companies.Name as Dep, dbo.Contacts.Prefix, dbo.Contacts.Contact, null as PersonName, null as PersonSurname, null as Department, Companies.Name as CompanyName, Contact_type as ContactType
FROM         dbo.Companies INNER JOIN
                      dbo.Company_contacts ON dbo.Companies.Company_ID = dbo.Company_contacts.Company_ID INNER JOIN
                      dbo.Contacts ON dbo.Company_contacts.Contact_ID = dbo.Contacts.Contact_ID INNER JOIN
                      dbo.Contact_directories ON dbo.Contacts.Contact_ID = dbo.Contact_directories.Contact_ID INNER JOIN
                      dbo.Contact_types ON dbo.Contacts.Contact_type_ID = dbo.Contact_types.Contact_type_ID INNER JOIN
                      dbo.Directories ON dbo.Contact_directories.Directory_ID = dbo.Directories.Directory_ID
WHERE				  ( 
						(Directories.Directory_ID = @DirId) AND
						(Companies.Name LIKE @Company + '%')
					  ) AND
					  (
						(Contact_types.Validation = 'T')
					  )

UNION


SELECT		 rtrim(ltrim(dbo.Persons.Surname + ' ' + dbo.Persons.Name)) as Nam, rtrim(dbo.Persons.Department + ', ' + dbo.Companies.Name) as Dep, dbo.Contacts.Prefix, dbo.Contacts.Contact, Persons.Name as PersonName, Persons.Surname as PersonSurname, Persons.Department as Department, Companies.Name as CompanyName, Contact_type as ContactType
FROM         dbo.Person_contacts INNER JOIN
                      dbo.Persons ON dbo.Person_contacts.Person_ID = dbo.Persons.Person_ID INNER JOIN
                      dbo.Contacts ON dbo.Person_contacts.Contact_ID = dbo.Contacts.Contact_ID INNER JOIN
                      dbo.Contact_directories ON dbo.Contacts.Contact_ID = dbo.Contact_directories.Contact_ID INNER JOIN
                      dbo.Directories ON dbo.Contact_directories.Directory_ID = dbo.Directories.Directory_ID INNER JOIN
                      dbo.Companies ON dbo.Persons.Company_ID = dbo.Companies.Company_ID INNER JOIN
                      dbo.Contact_types ON dbo.Contacts.Contact_type_ID = dbo.Contact_types.Contact_type_ID
WHERE				  ( (Persons.Name LIKE @Name + '%') AND
						(Persons.Surname LIKE @Surname + '%') AND
						(Persons.Department LIKE @Department + '%') AND
						(Directories.Directory_ID = @DirId) AND
						(Companies.Name LIKE @Company + '%')
					  ) AND
					  (
						(Contact_types.Validation = 'T')
					  )
ORDER BY Nam, Dep

GO
/****** Object:  StoredProcedure [dbo].[Get_local_directory_list]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Get_local_directory_list]



@Name VARCHAR(50),

@Surname VARCHAR(50),

@Department VARCHAR(50),

@DirId int



AS





SELECT         rtrim(ltrim(dbo.Persons.Surname + ' ' + dbo.Persons.Name)) as Nam, dbo.Persons.Department as Dep, dbo.Contacts.Prefix, dbo.Contacts.Contact

FROM dbo.Person_contacts INNER JOIN

dbo.Persons ON dbo.Person_contacts.Person_ID = dbo.Persons.Person_ID INNER JOIN

dbo.Contacts ON dbo.Person_contacts.Contact_ID = dbo.Contacts.Contact_ID INNER JOIN

dbo.Contact_directories ON dbo.Contacts.Contact_ID = dbo.Contact_directories.Contact_ID INNER JOIN

dbo.Directories ON dbo.Contact_directories.Directory_ID = dbo.Directories.Directory_ID INNER JOIN

dbo.Companies ON dbo.Persons.Company_ID = dbo.Companies.Company_ID INNER JOIN

dbo.Contact_types ON dbo.Contacts.Contact_type_ID = dbo.Contact_types.Contact_type_ID

WHERE                       ( (Persons.Name LIKE @Name + '%') AND

                                    (Persons.Surname LIKE @Surname + '%') AND

                                    (Persons.Department LIKE @Department + '%') AND

                                    (Directories.Directory_ID = @DirId)

                              ) AND

                              (

                                    (Contact_types.Validation = 'T')

                              )

ORDER BY Nam, Dep



GO
/****** Object:  StoredProcedure [dbo].[getCallerName]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[getCallerName]( @Calling_number varchar(50) )



AS

BEGIN



set @Calling_number = ltrim(rtrim(@Calling_number))



declare @Searched_ID int

declare @Row_CURSOR cursor



set @Row_CURSOR = cursor for

select Contact_ID, Contact, (len(Contact) - len( replace( Contact, '?','' ) )) as length

from contacts

where len(Contact) = len(@Calling_number)

order by length asc



open @Row_CURSOR

      declare @Current_number varchar(50)

      declare @Current_ID int

      declare @Current_length int

      --declare @Contact_compare_length int



      fetch next

      from @Row_CURSOR

      into @Current_ID, @Current_number, @Current_length

      

      while ( @@fetch_status = 0 )

      begin

            --print 'length: ' + cast(len(@Calling_number) - @Current_length + 1 AS varchar(20)) + ' ' + @Calling_number + ' current length: ' + cast(@Current_length as varchar(25))

            --print 'length: ' + cast(len(@Current_number) - @Current_length + 1 AS varchar(20)) + ' ' + @Current_number



            --set @Contact_compare_length = len(@Current_number) - @Current_length + 1

            --print substring(@Calling_number, 0, len(@Calling_number) - @Current_length + 1) + ' = ' + substring( @Current_number, 0, len(@Current_number) - @Current_length + 1 )



            if ( substring(@Calling_number, 0, len(@Calling_number) - @Current_length + 1) = substring( @Current_number, 0, len(@Current_number) - @Current_length + 1 ))

            begin

                  set @Searched_ID = @Current_ID

                  break

            end



            fetch next

            from @Row_CURSOR

            into @Current_ID, @Current_number, @Current_length

      end



close @Row_CURSOR

deallocate @Row_CURSOR



SELECT ltrim(coalesce(Companies.[Name],'') + ' ' + coalesce(Persons.[Name], '') + ' ' + coalesce(Persons.Surname, '')) AS [Name]

FROM Contacts LEFT OUTER JOIN Person_contacts ON Contacts.Contact_ID = Person_contacts.Contact_ID

                  LEFT OUTER JOIN Persons ON Person_contacts.Person_ID = Persons.Person_ID

                  LEFT OUTER JOIN Company_contacts ON Contacts.Contact_ID = Company_contacts.Contact_ID

                  LEFT OUTER JOIN Companies ON Company_contacts.Company_ID = Companies.Company_ID

WHERE Contacts.Contact_ID = @Searched_ID

GROUP BY Companies.[Name], Persons.[Name], Persons.Surname, Contact, Contacts.Contact_ID



END



GO
/****** Object:  StoredProcedure [dbo].[Import]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[Import] (
@Company as varchar(100),
@Firstname as varchar(50),
@Surname as varchar(50),
@Department as varchar(50),
@Contact as varchar(20),
@ContactType as int,
@DirectoryId as int,
@Prefix as varchar(10)
)as
/*declare @Company as varchar(100)
declare @Firstname as varchar(50)
declare @Surname as varchar(50)
declare @Contact as varchar(20)
declare @ContactType as int
declare @DirectoryId as int
declare @Prefix as varchar(10)

set @Company = 'Company 1'
set @Firstname = 'Ondrej'
set @Surname = 'Prochazka'
set @Contact = '1234'
set @ContactType = 1
set @DirectoryId = 7
set @Prefix = ''*/

--Company
declare @CompanyId as int
select @CompanyId=Company_ID from Companies where Name=@Company
if (@CompanyId is null)
begin
  insert into Companies (Name) values (@Company)
  set @CompanyId = @@identity
end
--select @CompanyId

--Insert Person
declare @personId as int
select @personId=Person_Id from Persons where Company_ID=@CompanyId and Name=@Firstname and Surname=@Surname and Department=@Department
if (@personId is null)
begin
  insert into persons (Company_ID,Name, Surname, Department, CreatedBy) values (@CompanyId, @Firstname, @Surname, @Department, 1)
  set @personId = @@identity

  --Insert Notes
  declare @noteId as int
  insert into Notes (Note) values ('')
  set @noteId = @@identity

  --Insert Person-Note
  insert into Person_notes (Person_ID, Note_ID) values (@personId, @noteId)
end

--Insert Contact
declare @contactId as int
select @contactId=Contact_ID from Contacts where Contact_type_Id=@ContactType and Contact=@Contact
if (@contactId is null)
begin
  insert into contacts (Contact_type_Id,Contact, Prefix, CreatedBy) values (@ContactType,@Contact, @prefix, 1)
  set @contactId = @@identity

  --Insert Person-Contact
  insert into Person_contacts (Person_Id, Contact_ID) values (@personId, @contactId)
end

if (select count(*) from Contact_directories where Contact_ID=@contactId and Directory_ID=@DirectoryId) = 0
begin
  --insert Contact-Directory
  insert into Contact_directories (Contact_ID,Directory_ID) VALUES (@contactId, @directoryId)
end


GO
/****** Object:  StoredProcedure [dbo].[Insert_companies_contacts]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_companies_contacts]



@Contact_type_ID int,

@Contact varchar(100),

@Prefix varchar(5),

@Company_ID int



AS



declare @Contact_ID int



INSERT INTO Contacts (Contact_type_ID, Contact, Prefix)

VALUES (@Contact_type_ID, @Contact, @Prefix)

SELECT @Contact_ID = @@identity



INSERT INTO Company_contacts (Company_ID, Contact_ID)

VALUES (@Company_ID, @Contact_ID)



GO
/****** Object:  StoredProcedure [dbo].[Insert_Company_Notes]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_Company_Notes]

@Name VARCHAR(100),
@Address VARCHAR(200),
@ICO VARCHAR(50),
@DIC VARCHAR(50),
@Note VARCHAR(250)

AS
BEGIN

DECLARE @Company_ID INT
DECLARE @Note_ID INT

INSERT INTO Companies (Name, Address, ICO, DIC) 
VALUES ( @Name, @Address, @ICO, @DIC)
SELECT @Company_ID = @@IDENTITY

INSERT INTO Notes (Note)
VALUES (@Note)
SELECT @Note_ID = @@IDENTITY

INSERT INTO Company_notes (Company_ID, Note_ID)
VALUES (@Company_ID, @Note_ID)

END


GO
/****** Object:  StoredProcedure [dbo].[Insert_directory]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[Insert_directory]



@Contact_ID int,

@Directory_ID int



AS

BEGIN

INSERT INTO Contact_directories (Contact_ID, Directory_ID)

VALUES (@Contact_ID, @Directory_ID)

END



GO
/****** Object:  StoredProcedure [dbo].[Insert_persons_contacts]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_persons_contacts]



@Contact_type_ID int,

@Contact varchar(100),

@Prefix varchar(5),

@Person_ID int



AS

BEGIN

declare @Contact_ID int



INSERT INTO Contacts (Contact_type_ID, Contact, Prefix)

VALUES (@Contact_type_ID, @Contact, @Prefix)

SELECT @Contact_ID = @@identity



INSERT INTO Person_contacts (Person_ID, Contact_ID)

VALUES (@Person_ID, @Contact_ID)



END



GO
/****** Object:  StoredProcedure [dbo].[Insert_Persons_Notes]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Insert_Persons_Notes]



@Company_Name varchar(100),

@Name varchar(50),

@Surname varchar(50),

@title_befor varchar(50),

@Position varchar(50),

@Note varchar(250),

@Department varchar(50)



AS

BEGIN



declare @Person_ID int

declare @Note_ID int

declare @Company_ID int



SET @Company_ID = (SELECT Company_ID FROM Companies WHERE Name = @Company_Name)



INSERT INTO Persons (Company_ID, Name, Surname, title_befor, Position, Department) 

VALUES (@Company_ID, @Name, @Surname, @title_befor, @Position, @Department)

select @Person_ID = @@identity



INSERT INTO Notes (Note)

VALUES (@Note)

select @Note_ID = @@identity



INSERT INTO Person_notes (Person_ID, Note_ID)

VALUES (@Person_ID, @Note_ID)



END



GO
/****** Object:  StoredProcedure [dbo].[Searching_companies_contacts]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[Searching_companies_contacts]

@Name varchar(100),
@Address varchar(200),
@ICO varchar(50),
@DIC varchar(50),
@Contact varchar(50),
@UserRole int,
@SortColumn varchar(50)
as

BEGIN

SET NOCOUNT ON;

DECLARE
@LocName varchar(100),
@LocAddress varchar(200),
@LocICO varchar(50),
@LocDIC varchar(50),
@LocContact varchar(50),
@LocUserRole int,
@LocSortColumn varchar(50),
@LocSortDirection varchar(50)

set @LocName = Replace(@Name, '*', '%') + '%'
set @LocAddress = Replace(@Address, '*', '%') + '%'
set @LocICO = Replace(@ICO, '*', '%') + '%'
set @LocDIC = Replace(@DIC, '*', '%') + '%'
set @LocContact = Replace(@Contact, '*', '%') + '%'
set @LocUserRole = @UserRole

IF (@SortColumn LIKE '%DESC%')
BEGIN
set @LocSortDirection = 'DESC'
set @LocSortColumn = LEFT(@SortColumn, CHARINDEX (' ', @SortColumn) - 1)
END
ELSE
BEGIN 
set @LocSortDirection = ''
set @LocSortColumn = @SortColumn
END


IF (@LocUserRole != 0 AND (@LocContact = '%' OR @LocContact = '%%'))
BEGIN
		--PRINT N'This user has SET NOCOUNT turned ON.' + @LocName + ', ' +@LocAddress + ', ' +@LocICO+ ', ' +@LocDIC+ ', ' +@LocContact +', ';
        SELECT TOP 1000 [Company_ID], [Name], [Address], [ICO], [DIC], [Note] 
		FROM [Company_notes_view] 
		WHERE ([Name] LIKE @LocName AND [Address] LIKE @LocAddress AND [ICO] LIKE @LocICO AND [DIC] LIKE @LocDIC) 
		ORDER BY  
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = '' THEN [Name] END ASC, 
		CASE WHEN @LocSortColumn = 'Address' AND @LocSortDirection = '' THEN [Address] END ASC,
		CASE WHEN @LocSortColumn = 'ICO' AND @LocSortDirection = '' THEN [ICO] END ASC,
		CASE WHEN @LocSortColumn = 'DIC' AND @LocSortDirection = '' THEN [DIC] END ASC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = '' THEN [Note] END ASC,
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = 'DESC' THEN [Name] END DESC,
		CASE WHEN @LocSortColumn = 'Address' AND @LocSortDirection = 'DESC' THEN [Address] END DESC,
		CASE WHEN @LocSortColumn = 'ICO' AND @LocSortDirection = 'DESC' THEN [ICO] END DESC,
		CASE WHEN @LocSortColumn = 'DIC' AND @LocSortDirection = 'DESC' THEN [DIC] END DESC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = 'DESC' THEN [Note]
		ELSE [Name]
		 END DESC
		OPTION (RECOMPILE)
END
ELSE IF (@LocUserRole = 0 AND (@LocContact = '%' OR @LocContact = '%%')) 
BEGIN
        SELECT TOP 1000 [Company_ID], [Name], [Address], [ICO], [DIC], [Note] 
		FROM [Company_notes_view_restricted] 
		WHERE [Name] LIKE @LocName AND [Address] LIKE @LocAddress AND [ICO] LIKE @LocICO AND [DIC] LIKE @LocDIC 
		ORDER BY  
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = '' THEN [Name] END ASC, 
		CASE WHEN @LocSortColumn = 'Address' AND @LocSortDirection = '' THEN [Address] END ASC,
		CASE WHEN @LocSortColumn = 'ICO' AND @LocSortDirection = '' THEN [ICO] END ASC,
		CASE WHEN @LocSortColumn = 'DIC' AND @LocSortDirection = '' THEN [DIC] END ASC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = '' THEN [Note] END ASC,
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = 'DESC' THEN [Name] END DESC,
		CASE WHEN @LocSortColumn = 'Address' AND @LocSortDirection = 'DESC' THEN [Address] END DESC,
		CASE WHEN @LocSortColumn = 'ICO' AND @LocSortDirection = 'DESC' THEN [ICO] END DESC,
		CASE WHEN @LocSortColumn = 'DIC' AND @LocSortDirection = 'DESC' THEN [DIC] END DESC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = 'DESC' THEN [Note]
		ELSE [Name]
		 END DESC
		OPTION (RECOMPILE)
END
ELSE IF (@LocUserRole != 0)
BEGIN
		SELECT TOP 1000 [Company_ID], [Name], [Address], [ICO], [DIC], [Note] 
		FROM [Company_notes_view] 
		WHERE
		 [Name] LIKE @LocName 
		   aND [Address] LIKE @Locaddress
		   AND [ICO] LIKE @LocICO
		   AND [DIC] LIKE @LocDIC
		   AND [Company_ID] IN (SELECT [Company_ID] FROM [Companies_contacts_notes_view] WHERE [Contact] LIKE @LocContact)
		ORDER BY  
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = '' THEN [Name] END ASC, 
		CASE WHEN @LocSortColumn = 'Address' AND @LocSortDirection = '' THEN [Address] END ASC,
		CASE WHEN @LocSortColumn = 'ICO' AND @LocSortDirection = '' THEN [ICO] END ASC,
		CASE WHEN @LocSortColumn = 'DIC' AND @LocSortDirection = '' THEN [DIC] END ASC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = '' THEN [Note] END ASC,
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = 'DESC' THEN [Name] END DESC,
		CASE WHEN @LocSortColumn = 'Address' AND @LocSortDirection = 'DESC' THEN [Address] END DESC,
		CASE WHEN @LocSortColumn = 'ICO' AND @LocSortDirection = 'DESC' THEN [ICO] END DESC,
		CASE WHEN @LocSortColumn = 'DIC' AND @LocSortDirection = 'DESC' THEN [DIC] END DESC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = 'DESC' THEN [Note]
		ELSE [Name]
		 END DESC
		OPTION (RECOMPILE)
END
ELSE		
BEGIN
		SELECT TOP 1000 [Company_ID], [Name], [Address], [ICO], [DIC], [Note] 
		FROM [Company_notes_view_restricted] 
		WHERE [Name] LIKE @LocName AND [Address] LIKE @LocAddress AND [ICO] LIKE @LocICO AND [DIC] LIKE @LocDIC AND 
		[Company_ID] IN (SELECT [Company_ID] FROM [Companies_contacts_notes_view_restricted] WHERE [Contact] LIKE @LocContact) 
		ORDER BY  
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = '' THEN [Name] END ASC, 
		CASE WHEN @LocSortColumn = 'Address' AND @LocSortDirection = '' THEN [Address] END ASC,
		CASE WHEN @LocSortColumn = 'ICO' AND @LocSortDirection = '' THEN [ICO] END ASC,
		CASE WHEN @LocSortColumn = 'DIC' AND @LocSortDirection = '' THEN [DIC] END ASC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = '' THEN [Note] END ASC,
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = 'DESC' THEN [Name] END DESC,
		CASE WHEN @LocSortColumn = 'Address' AND @LocSortDirection = 'DESC' THEN [Address] END DESC,
		CASE WHEN @LocSortColumn = 'ICO' AND @LocSortDirection = 'DESC' THEN [ICO] END DESC,
		CASE WHEN @LocSortColumn = 'DIC' AND @LocSortDirection = 'DESC' THEN [DIC] END DESC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = 'DESC' THEN [Note]
		ELSE [Name]
		 END DESC
		OPTION (RECOMPILE)
END
END








GO
/****** Object:  StoredProcedure [dbo].[Searching_persons_contacts]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[Searching_persons_contacts]

@Name varchar(50),
@Surname varchar(50),
@Contact varchar(50),
@Company_Name varchar(100),
@Department varchar(50),
@UserRole int,
@SortColumn varchar(50)
as

DECLARE
@LocName varchar(50),
@LocSurname varchar(50),
@LocContact varchar(50),
@LocCompany_Name varchar(100),
@LocDepartment varchar(50),
@LocUserRole int,
@LocSortColumn varchar(50),
@LocSortDirection varchar(50)


set @LocName = Replace(@Name, '*', '%') + '%'
set @LocSurname = Replace(@Surname, '*', '%') + '%'
set @LocContact = Replace(@Contact, '*', '%') + '%'
set @LocCompany_Name = Replace(@Company_Name, '*', '%') + '%'
set @LocDepartment = Replace(@Department, '*', '%') + '%'
set @LocUserRole = @UserRole


BEGIN

IF (@SortColumn LIKE '%DESC%')
BEGIN
set @LocSortDirection = 'DESC'
set @LocSortColumn = LEFT(@SortColumn, CHARINDEX (' ', @SortColumn) - 1)
END
ELSE
BEGIN 
set @LocSortDirection = ''
set @LocSortColumn = @SortColumn
END

	SET NOCOUNT ON;

IF (@LocUserRole != 0 AND (@LocContact = '%' OR @LocContact = '%%'))
BEGIN

      PRINT @LocSortDirection+@LocSortColumn
	  SELECT TOP 1000 [Name], [Surname], [Person_ID], [Company_Name], [Position], [Note], [Department] 
	  FROM [Persons_searching_view] 
	  WHERE [Company_Name] LIKE @LocCompany_Name AND [Name] LIKE @LocName AND [Surname] LIKE @LocSurname AND [Department] LIKE @LocDepartment 
	  ORDER BY  
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = '' THEN [Name] END ASC, 
		CASE WHEN @LocSortColumn = 'Surname' AND @LocSortDirection = '' THEN [Surname] END ASC,
		CASE WHEN @LocSortColumn = 'Company_Name' AND @LocSortDirection = '' THEN [Company_Name] END ASC,
		CASE WHEN @LocSortColumn = 'Position' AND @LocSortDirection = '' THEN [POSITION] END ASC,
		CASE WHEN @LocSortColumn = 'Department' AND @LocSortDirection = '' THEN [Department] END ASC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = '' THEN [Note] END ASC,
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = 'DESC' THEN [Name] END DESC, 
		CASE WHEN @LocSortColumn = 'Surname' AND @LocSortDirection = 'DESC' THEN [Surname] END DESC,
		CASE WHEN @LocSortColumn = 'Company_Name' AND @LocSortDirection = 'DESC' THEN [Company_Name] END DESC,
		CASE WHEN @LocSortColumn = 'Position' AND @LocSortDirection = 'DESC' THEN [POSITION] END DESC,
		CASE WHEN @LocSortColumn = 'Department' AND @LocSortDirection = 'DESC' THEN [Department] END DESC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = 'DESC' THEN [Note]
		ELSE [Surname]
		 END DESC
	  OPTION (RECOMPILE)
END
ELSE IF (@LocUserRole = 0 AND (@LocContact = '%' OR @LocContact = '%%'))
BEGIN

     SELECT TOP 1000 [Name], [Surname], [Person_ID], [Company_Name], [Position], [Note], [Department] 
	 FROM [Persons_companies_contacts_notes_view_restricted] 
	 WHERE [Company_Name] LIKE @LocCompany_Name AND [Name] LIKE @LocName AND [Surname] LIKE @LocSurname AND [Department] LIKE @LocDepartment 
	 GROUP BY [Name], [Surname], [Person_ID], [Company_Name], [Position], [Note], [Department] 
	 ORDER BY  
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = '' THEN [Name] END ASC, 
		CASE WHEN @LocSortColumn = 'Surname' AND @LocSortDirection = '' THEN [Surname] END ASC,
		CASE WHEN @LocSortColumn = 'Company_Name' AND @LocSortDirection = '' THEN [Company_Name] END ASC,
		CASE WHEN @LocSortColumn = 'Position' AND @LocSortDirection = '' THEN [POSITION] END ASC,
		CASE WHEN @LocSortColumn = 'Department' AND @LocSortDirection = '' THEN [Department] END ASC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = '' THEN [Note] END ASC,
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = 'DESC' THEN [Name] END DESC, 
		CASE WHEN @LocSortColumn = 'Surname' AND @LocSortDirection = 'DESC' THEN [Surname] END DESC,
		CASE WHEN @LocSortColumn = 'Company_Name' AND @LocSortDirection = 'DESC' THEN [Company_Name] END DESC,
		CASE WHEN @LocSortColumn = 'Position' AND @LocSortDirection = 'DESC' THEN [POSITION] END DESC,
		CASE WHEN @LocSortColumn = 'Department' AND @LocSortDirection = 'DESC' THEN [Department] END DESC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = 'DESC' THEN [Note]
		ELSE [Surname]
		 END DESC
	 OPTION (RECOMPILE)
END
ELSE IF (@LocUserRole != 0)
BEGIN

	 SELECT TOP 1000 [Name], [Surname], [Person_ID], [Company_Name], [Position], [Note], [Department] 
	 FROM [Persons_companies_contacts_notes_view] 
	 WHERE [Company_Name] LIKE @LocCompany_Name AND [Name] LIKE @LocName AND [Surname] LIKE @LocSurname AND [Contact] LIKE @LocContact AND [Department] LIKE @LocDepartment 
	 GROUP BY [Name], [Surname], [Person_ID], [Company_Name], [Position], [Note], [Department] 
	   ORDER BY  
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = '' THEN [Name] END ASC, 
		CASE WHEN @LocSortColumn = 'Surname' AND @LocSortDirection = '' THEN [Surname] END ASC,
		CASE WHEN @LocSortColumn = 'Company_Name' AND @LocSortDirection = '' THEN [Company_Name] END ASC,
		CASE WHEN @LocSortColumn = 'Position' AND @LocSortDirection = '' THEN [POSITION] END ASC,
		CASE WHEN @LocSortColumn = 'Department' AND @LocSortDirection = '' THEN [Department] END ASC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = '' THEN [Note] END ASC,
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = 'DESC' THEN [Name] END DESC, 
		CASE WHEN @LocSortColumn = 'Surname' AND @LocSortDirection = 'DESC' THEN [Surname] END DESC,
		CASE WHEN @LocSortColumn = 'Company_Name' AND @LocSortDirection = 'DESC' THEN [Company_Name] END DESC,
		CASE WHEN @LocSortColumn = 'Position' AND @LocSortDirection = 'DESC' THEN [POSITION] END DESC,
		CASE WHEN @LocSortColumn = 'Department' AND @LocSortDirection = 'DESC' THEN [Department] END DESC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = 'DESC' THEN [Note]
		ELSE [Surname]
		 END DESC
	 OPTION (RECOMPILE)
END
ELSE
BEGIN 

	 SELECT TOP 1000 [Name], [Surname], [Person_ID], [Company_Name], [Position], [Note], [Department] 
	 FROM [Persons_companies_contacts_notes_view_restricted] 
	 WHERE [Company_Name] LIKE @LocCompany_Name AND [Name] LIKE @LocName AND [Surname] LIKE @LocSurname AND [Contact] LIKE @LocContact AND [Department] LIKE @LocDepartment 
	 GROUP BY [Name], [Surname], [Person_ID], [Company_Name], [Position], [Note], [Department] 
	 ORDER BY  
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = '' THEN [Name] END ASC, 
		CASE WHEN @LocSortColumn = 'Surname' AND @LocSortDirection = '' THEN [Surname] END ASC,
		CASE WHEN @LocSortColumn = 'Company_Name' AND @LocSortDirection = '' THEN [Company_Name] END ASC,
		CASE WHEN @LocSortColumn = 'Position' AND @LocSortDirection = '' THEN [POSITION] END ASC,
		CASE WHEN @LocSortColumn = 'Department' AND @LocSortDirection = '' THEN [Department] END ASC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = '' THEN [Note] END ASC,
		CASE WHEN @LocSortColumn = 'Name' AND @LocSortDirection = 'DESC' THEN [Name] END DESC, 
		CASE WHEN @LocSortColumn = 'Surname' AND @LocSortDirection = 'DESC' THEN [Surname] END DESC,
		CASE WHEN @LocSortColumn = 'Company_Name' AND @LocSortDirection = 'DESC' THEN [Company_Name] END DESC,
		CASE WHEN @LocSortColumn = 'Position' AND @LocSortDirection = 'DESC' THEN [POSITION] END DESC,
		CASE WHEN @LocSortColumn = 'Department' AND @LocSortDirection = 'DESC' THEN [Department] END DESC,
		CASE WHEN @LocSortColumn = 'Note' AND @LocSortDirection = 'DESC' THEN [Note]
		ELSE [Surname]
		 END DESC
	 OPTION (RECOMPILE)
END
END



GO
/****** Object:  StoredProcedure [dbo].[Select_Company_Contacts]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Select_Company_Contacts]

	@Company_Id int, 
	@UserRole int

AS

DECLARE
@LocCompanyID varchar(50),
@LocUserRole int

set @LocCompanyID = @Company_Id
set @LocUserRole = @UserRole

BEGIN
	SET NOCOUNT ON;

	IF (@LocUserRole != 0)
	
	BEGIN
	SELECT Contact, Contact_type, Validation 
	FROM Companies_contacts_notes_view 
	WHERE (Company_ID = @LocCompanyID) AND (Contact IS NOT NULL)
	OPTION (RECOMPILE)
	END
	
	ELSE IF (@LocUserRole = 0)
	
	BEGIN
	SELECT Contact, Contact_type, Validation 
	FROM Companies_contacts_notes_view_restricted 
	WHERE (Company_ID = @LocCompanyID) AND (Contact IS NOT NULL)
	OPTION (RECOMPILE)
	END
END


GO
/****** Object:  StoredProcedure [dbo].[Select_Person_Contacts]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Select_Person_Contacts] 
-- Add the parameters for the stored procedure here
@Person_ID int, 
@UserRole int

AS

BEGIN

SET NOCOUNT ON;

DECLARE
@LocPersonID varchar(50),
@LocUserRole int

set @LocPersonID = @Person_ID
set @LocUserRole = @UserRole


 IF (@LocUserRole != 0)
 BEGIN
		SELECT [Contact_type], [Contact] 
		FROM [Persons_companies_contacts_notes_view] 
		WHERE (([Person_ID] = @LocPersonID ) AND ([Contact] IS NOT NULL))
		OPTION (RECOMPILE)
 END
ELSE IF (@LocUserRole = 0)
BEGIN
		SELECT [Contact_type], [Contact] 
		FROM [Select_person_contacts_view_restricted] 
		WHERE (([Person_ID] = @LocPersonID ) AND ([Contact] IS NOT NULL))
		OPTION (RECOMPILE)
END
END


GO
/****** Object:  StoredProcedure [dbo].[TestingProcedure]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TestingProcedure]
	-- Add the parameters for the stored procedure here
	@sent_mail_id as int,

     

      @no_ as varchar(50),

      @mail_subject as varchar(150),

      @text as varchar(3000),

      @to_address as varchar(300),

      @cc_address as varchar(300),

      @bcc_address as varchar(300),

      @error_address as varchar(300),

      @attach as varchar(500) = null,

      @file_query as varchar(3000) = null,

      @is_debugging as bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * From INFORMATION_SCHEMA.TABLE_PRIVILEGES;
END

GO
/****** Object:  StoredProcedure [dbo].[TestingProcedureNow]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TestingProcedureNow]
	-- Add the parameters for the stored procedure here
	@sent_mail_id as int,

      @mailing_id as int,

      @no_ as varchar(50),

      @mail_subject as varchar(150),

      @text as varchar(3000),

      @to_address as varchar(300),

      @cc_address as varchar(300),

      @bcc_address as varchar(300),

      @error_address as varchar(300),

      @attach as varchar(500) = null,

      @file_query as varchar(3000) = null,

      @is_debugging as bit = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * From INFORMATION_SCHEMA.TABLE_PRIVILEGES;
END

GO
/****** Object:  StoredProcedure [dbo].[Update_companies_notes]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_companies_notes]

@Name VARCHAR(100),
@Address VARCHAR(200),
@ICO VARCHAR(50),
@DIC VARCHAR(50),
@Company_ID INT,
@Note VARCHAR(250),
@Note_ID INT

AS
BEGIN
 
UPDATE Companies
SET Name = @Name, Address = @Address, ICO = @ICO, DIC = @DIC
WHERE Company_ID = @Company_ID

UPDATE Notes
SET Note = @Note
WHERE Note_ID = @Note_ID

END


GO
/****** Object:  StoredProcedure [dbo].[Update_contacts]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_contacts]

@Contact_ID INT,
@Contact VARCHAR(100),
@Contact_type_ID SMALLINT,
@Prefix VARCHAR(5)

AS

BEGIN

UPDATE Contacts
SET Contact = @Contact, Contact_type_ID = @Contact_type_ID +1, Prefix = @Prefix
WHERE Contact_ID = @Contact_ID

END


GO
/****** Object:  StoredProcedure [dbo].[Update_persons_notes]    Script Date: 07.03.2017 16:36:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Update_persons_notes]

@Name VARCHAR(50),
@Surname VARCHAR(50),
@title_befor VARCHAR(50),
@POSITION VARCHAR(50),
@Person_ID INT,
@Note VARCHAR(250),
@Note_ID INT,
@Company_Name VARCHAR(100),
@Department VARCHAR(50)

AS

DECLARE @Company_ID INT

SET @Company_ID = (SELECT Company_ID FROM Companies WHERE Name = @Company_Name)

UPDATE Persons
SET Company_ID = @Company_ID, Name = @Name, Surname = @Surname, title_befor = @title_befor, POSITION = @POSITION, Department = @Department
WHERE Person_ID = @Person_ID

UPDATE Notes
SET Note = @Note
WHERE Note_ID = @Note_ID


GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Cars"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_Cars'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'View_Cars'
GO
