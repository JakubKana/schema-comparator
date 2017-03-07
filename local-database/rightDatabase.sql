USE [DatabaseComparatorTest2]
GO
/****** Object:  Table [dbo].[CarPerson]    Script Date: 07.03.2017 16:51:10 ******/
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
/****** Object:  Table [dbo].[Cars2]    Script Date: 07.03.2017 16:51:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cars2](
	[ID] [int] NOT NULL,
	[Name] [varbinary](50) NULL,
	[Price] [money] NULL,
 CONSTRAINT [PK_Cars2] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PersoN]    Script Date: 07.03.2017 16:51:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersoN](
	[ID] [int] NOT NULL,
	[Awesome] [nchar](10) NULL,
	[Name] [nchar](10) NULL,
	[Surname] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Product]    Script Date: 07.03.2017 16:51:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ID] [int] NULL,
	[Name] [varchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  UserDefinedFunction [dbo].[LUDACRIS]    Script Date: 07.03.2017 16:51:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[LUDACRIS] (@storeid int)  
RETURNS TABLE  
AS  
RETURN   
(  
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS
);  

GO
/****** Object:  View [dbo].[View_Cars]    Script Date: 07.03.2017 16:51:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[View_Cars]
AS
SELECT        Name, Price
FROM            dbo.Cars2

GO
ALTER TABLE [dbo].[CarPerson]  WITH CHECK ADD  CONSTRAINT [FK_CarPerson_PersoN] FOREIGN KEY([PersonId])
REFERENCES [dbo].[PersoN] ([ID])
GO
ALTER TABLE [dbo].[CarPerson] CHECK CONSTRAINT [FK_CarPerson_PersoN]
GO
ALTER TABLE [dbo].[PersoN]  WITH CHECK ADD  CONSTRAINT [CK_PersoN] CHECK  (([ID]>=(0) AND [ID]<=(10000)))
GO
ALTER TABLE [dbo].[PersoN] CHECK CONSTRAINT [CK_PersoN]
GO
/****** Object:  StoredProcedure [dbo].[Companies_contact_by_Company_ID]    Script Date: 07.03.2017 16:51:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure[dbo].[Companies_contact_by_Company_ID] @Company_ID int as select * from Companies_contact_view where Company_ID = @Company_ID
GO
/****** Object:  StoredProcedure [dbo].[TestingProcedure]    Script Date: 07.03.2017 16:51:10 ******/
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
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[39] 4[35] 2[13] 3) )"
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
         Begin Table = "Cars2"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 177
               Right = 256
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
