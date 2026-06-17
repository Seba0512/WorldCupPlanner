INSERT INTO WorldCupPlanner.dbo.Auditorias ([Timestamp],Usuario,Accion,Detalle) VALUES
	 (N'2026-06-17T21:39:31.3043987Z',N'administrador@mail.com',N'LOGIN_EXITOSO',N'Login exitoso: administrador@mail.com'),
	 (N'2026-06-17T21:39:52.7279617Z',N'administrador@mail.com',N'LOGIN_EXITOSO',N'Login exitoso: administrador@mail.com'),
	 (N'2026-06-17T21:40:14.5487705Z',N'administrador@mail.com',N'LOGIN_EXITOSO',N'Login exitoso: administrador@mail.com');
INSERT INTO WorldCupPlanner.dbo.Usuarios (Email,Nombre,Apellido,FechaNacimiento,DebeCambiarContrasenia,Roles,ContraseniaCifrada) VALUES
	 (N'administrador@mail.com',N'Administrador',N'Sistema','1990-01-01 00:00:00.0000000',0,N'AdministradorDelSistema',N'7tBaVvpQyjohfslzcnOLu8UxXHrAcYgmdywd8+AuNAY='),
	 (N'administrador2@mail.com',N'Administrador2',N'Sistema','2008-06-17 00:00:00.0000000',0,N'AdministradorDelSistema',N'02cHgsLLnuLCgoS3zybEPt7Q/5NPHtbJw/xY/nbmJ9A='),
	 (N'editor@mail.com',N'Editor',N'Sistema','2008-06-17 00:00:00.0000000',0,N'Editor',N'nDREYNhwKZh6Pqp1jJ+FOgK/jvmUr00EmUaqKkV/kHQ='),
	 (N'editor2@mail.com',N'Editor2',N'Sistema','2008-06-17 00:00:00.0000000',0,N'Editor',N'eLSB/KFG5qEuTdB4ckdZIp2H/uQKuORo5+7xqmgYGZQ='),
	 (N'periodista@mail.com',N'Periodista',N'Sistema','2008-06-17 00:00:00.0000000',0,N'Periodista',N'9VwX9o+6A+hJc9Uxt0Xxn5/E8I/f68q+1pD34dqhR+8='),
	 (N'periodista2@mail.com',N'Periodista2',N'Sistema','2008-06-17 00:00:00.0000000',0,N'Periodista',N'Ps9Lw1qSz+XJhK4phw+mk0M8nO8mhHIxioPExsndUS4='),
	 (N'admineditorperiodista@mail.com',N'AdminEditorPeriodista',N'Sistema','2008-06-17 00:00:00.0000000',0,N'AdministradorDelSistema,Editor,Periodista',N'tq9OlT6HFvY8vTM3iFL9cvlygIBieJVhRYDnpYeDBD8=');
