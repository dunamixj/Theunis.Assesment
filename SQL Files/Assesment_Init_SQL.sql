 IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'Assesment')
	BEGIN
    CREATE DATABASE [Assesment]
		 CONTAINMENT = NONE
		 ON  PRIMARY 
		( NAME = N'Assesment', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\Assesment.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
		 LOG ON 
		( NAME = N'Assesment_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\Assesment_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
		 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
	
	IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
	begin
		EXEC [Assesment].[dbo].[sp_fulltext_database] @action = 'enable'
	end
	END

	BEGIN
		/****** Object:  Login [TJvV]    Script Date: 2024-11-23 3:55:15 PM ******/
		CREATE LOGIN [TJvV] WITH PASSWORD=N'@ss3sm3nt', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
	END
	
	BEGIN
		USE [Assesment]
		/****** Object:  User [TJvV]    Script Date: 2024-11-23 3:59:37 PM ******/
		CREATE USER [TJvV] FOR LOGIN [TJvV] WITH DEFAULT_SCHEMA=[dbo]
	END

	BEGIN
		USE [Assesment]
		--You need to check if the table exists
		IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Transaction' and xtype='U')
		
			/****** Object:  Table [dbo].[Transaction]    Script Date: 2024-11-23 5:35:20 PM ******/
			SET ANSI_NULLS ON
			SET QUOTED_IDENTIFIER ON
			
			CREATE TABLE [dbo].[Transaction](
				[Id] [int] IDENTITY(1,1) NOT NULL,
				[TransactionId] [varchar](50) NOT NULL,
				[AccountNumber] [varchar](30) NOT NULL,
				[Amount] [decimal](18, 2) NOT NULL,
				[CurrencyCode] [varchar](3) NOT NULL,
				[TransactionDate] [datetime] NOT NULL,
				[Status] [varchar](10) NOT NULL,
				[OutputStatus] [varchar](1) NOT NULL,
			 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
			(
				[Id] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
			) ON [PRIMARY]
	END
