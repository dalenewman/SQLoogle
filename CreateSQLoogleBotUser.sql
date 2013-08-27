USE master;

IF NOT EXISTS (
		SELECT *
		FROM master.sys.server_principals
		WHERE name = 'SQLoogleBot'
)	BEGIN
		CREATE LOGIN [SQLoogleBot] WITH PASSWORD=N'password', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF;
		GRANT VIEW ANY DATABASE TO [SQLoogleBot];
		GRANT VIEW ANY DEFINITION TO [SQLoogleBot];
		GRANT VIEW SERVER STATE TO [SQLoogleBot];
		PRINT 'Created Login with appropriate server level rights';
	END

-- NOTE: Once you've created SQLoogleBot in model database, it should already be in any new databases.
EXECUTE master.sys.sp_MSforeachdb '
	USE [?];
	IF DB_NAME() != ''tempdb'' AND NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N''SQLoogleBot'')
		BEGIN
			CREATE USER [SQLoogleBot] FOR LOGIN [SQLoogleBot];
			PRINT ''User Created in '' + DB_NAME();
		END
		
	EXEC sp_change_users_login ''Auto_Fix'', ''SQLoogleBot'';
		
	IF DATABASE_PRINCIPAL_ID(''RSExecRole'') IS NOT NULL AND OBJECT_ID(''Catalog'') IS NOT NULL
		BEGIN
			GRANT SELECT ON [dbo].[Catalog] TO [SQLoogleBot];
			PRINT ''Granted Permission to Reporting Services Jobs'';
		END
		
	IF DB_NAME() = ''msdb''
		BEGIN
			GRANT SELECT ON [dbo].[sysjobs] TO [SQLoogleBot];
			GRANT SELECT ON [dbo].[sysjobsteps] TO [SQLoogleBot];
			PRINT ''Granted Permission to SQL Agent Jobs'';
		END
'