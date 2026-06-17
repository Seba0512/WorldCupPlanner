-- DROP SCHEMA dbo;

-- WorldCupPlanner.dbo.Auditorias definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Auditorias;

CREATE TABLE WorldCupPlanner.dbo.Auditorias (
	Id int IDENTITY(1,1) NOT NULL,
	[Timestamp] nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Usuario nvarchar(400) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Accion nvarchar(400) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Detalle nvarchar(2000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_Auditorias PRIMARY KEY (Id)
);


-- WorldCupPlanner.dbo.ConfiguracionesTorneo definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.ConfiguracionesTorneo;

CREATE TABLE WorldCupPlanner.dbo.ConfiguracionesTorneo (
	Id int NOT NULL,
	SemillaFixture int NOT NULL,
	SemillaCompletar int NOT NULL,
	SemillaCrucesFase int NOT NULL,
	SemillaSimulation int NOT NULL,
	FechaInicioTorneo datetime2 NOT NULL,
	MaxPartidosPorDia int NOT NULL,
	SeparacionEntreFechas int NOT NULL,
	HoraInicioPartidos bigint NOT NULL,
	SeparacionHorasEntrePartidos int NOT NULL,
	MotorSimulacion int DEFAULT 0 NOT NULL,
	CONSTRAINT PK_ConfiguracionesTorneo PRIMARY KEY (Id)
);


-- WorldCupPlanner.dbo.Equipos definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Equipos;

CREATE TABLE WorldCupPlanner.dbo.Equipos (
	Id int IDENTITY(1,1) NOT NULL,
	Nombre nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	RankingFIFA int NOT NULL,
	Confederacion int NOT NULL,
	Bandera nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	RankingInicial int DEFAULT 0 NOT NULL,
	CONSTRAINT PK_Equipos PRIMARY KEY (Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Equipos_Nombre ON WorldCupPlanner.dbo.Equipos (  Nombre ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlanner.dbo.Estadios definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Estadios;

CREATE TABLE WorldCupPlanner.dbo.Estadios (
	Id int IDENTITY(1,1) NOT NULL,
	Nombre nvarchar(160) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Ciudad nvarchar(120) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Capacidad int NOT NULL,
	Descripcion nvarchar(800) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_Estadios PRIMARY KEY (Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Estadios_Nombre ON WorldCupPlanner.dbo.Estadios (  Nombre ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlanner.dbo.Grupos definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Grupos;

CREATE TABLE WorldCupPlanner.dbo.Grupos (
	Id int IDENTITY(1,1) NOT NULL,
	Etiqueta nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_Grupos PRIMARY KEY (Id)
);


-- WorldCupPlanner.dbo.Notificaciones definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Notificaciones;

CREATE TABLE WorldCupPlanner.dbo.Notificaciones (
	Id int IDENTITY(1,1) NOT NULL,
	Mensaje nvarchar(1000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Timestamp] datetime2 NOT NULL,
	UsuarioDestinatarioId int NOT NULL,
	EsLeida bit NOT NULL,
	CONSTRAINT PK_Notificaciones PRIMARY KEY (Id)
);


-- WorldCupPlanner.dbo.TorneosArchivados definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.TorneosArchivados;

CREATE TABLE WorldCupPlanner.dbo.TorneosArchivados (
	Id int IDENTITY(1,1) NOT NULL,
	GruposJson nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	PartidosEliminatoriosJson nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_TorneosArchivados PRIMARY KEY (Id)
);


-- WorldCupPlanner.dbo.Usuarios definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Usuarios;

CREATE TABLE WorldCupPlanner.dbo.Usuarios (
	Email nvarchar(400) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Nombre nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Apellido nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	FechaNacimiento datetime2 NOT NULL,
	DebeCambiarContrasenia bit NOT NULL,
	Roles nvarchar(400) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ContraseniaCifrada nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Id int IDENTITY(1,1) NOT NULL,
	CONSTRAINT PK_Usuarios PRIMARY KEY (Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Usuarios_Email ON WorldCupPlanner.dbo.Usuarios (  Email ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlanner.dbo.[__EFMigrationsHistory] definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.[__EFMigrationsHistory];

CREATE TABLE WorldCupPlanner.dbo.[__EFMigrationsHistory] (
	MigrationId nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ProductVersion nvarchar(64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
);


-- WorldCupPlanner.dbo.GrupoEquipo definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.GrupoEquipo;

CREATE TABLE WorldCupPlanner.dbo.GrupoEquipo (
	EquiposId int NOT NULL,
	GrupoId int NOT NULL,
	CONSTRAINT PK_GrupoEquipo PRIMARY KEY (EquiposId,GrupoId),
	CONSTRAINT FK_GrupoEquipo_Equipos_EquiposId FOREIGN KEY (EquiposId) REFERENCES WorldCupPlanner.dbo.Equipos(Id) ON DELETE CASCADE,
	CONSTRAINT FK_GrupoEquipo_Grupos_GrupoId FOREIGN KEY (GrupoId) REFERENCES WorldCupPlanner.dbo.Grupos(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_GrupoEquipo_GrupoId ON WorldCupPlanner.dbo.GrupoEquipo (  GrupoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlanner.dbo.Incidencia definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Incidencia;

CREATE TABLE WorldCupPlanner.dbo.Incidencia (
	Id int IDENTITY(1,1) NOT NULL,
	MinutoDelPartido int NOT NULL,
	EquipoId int NULL,
	PartidoId int NULL,
	Tipo nvarchar(42) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_Incidencia PRIMARY KEY (Id),
	CONSTRAINT FK_Incidencia_Equipos_EquipoId FOREIGN KEY (EquipoId) REFERENCES WorldCupPlanner.dbo.Equipos(Id)
);
 CREATE NONCLUSTERED INDEX IX_Incidencia_EquipoId ON WorldCupPlanner.dbo.Incidencia (  EquipoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Incidencia_PartidoId ON WorldCupPlanner.dbo.Incidencia (  PartidoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- WorldCupPlanner.dbo.Partidos definition

-- Drop table

-- DROP TABLE WorldCupPlanner.dbo.Partidos;

CREATE TABLE WorldCupPlanner.dbo.Partidos (
	Id int NOT NULL,
	Fecha datetime2 NOT NULL,
	EstadioId int NULL,
	EquipoLocalId int NULL,
	EquipoVisitanteId int NULL,
	Fase int NOT NULL,
	GolesLocal int NULL,
	GolesVisitante int NULL,
	GrupoId int NULL,
	GanadorPorPenalesId int NULL,
	CONSTRAINT PK_Partidos PRIMARY KEY (Id),
	CONSTRAINT FK_Partidos_Equipos_EquipoLocalId FOREIGN KEY (EquipoLocalId) REFERENCES WorldCupPlanner.dbo.Equipos(Id),
	CONSTRAINT FK_Partidos_Equipos_EquipoVisitanteId FOREIGN KEY (EquipoVisitanteId) REFERENCES WorldCupPlanner.dbo.Equipos(Id),
	CONSTRAINT FK_Partidos_Equipos_GanadorPorPenalesId FOREIGN KEY (GanadorPorPenalesId) REFERENCES WorldCupPlanner.dbo.Equipos(Id),
	CONSTRAINT FK_Partidos_Estadios_EstadioId FOREIGN KEY (EstadioId) REFERENCES WorldCupPlanner.dbo.Estadios(Id),
	CONSTRAINT FK_Partidos_Grupos_GrupoId FOREIGN KEY (GrupoId) REFERENCES WorldCupPlanner.dbo.Grupos(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_Partidos_EquipoLocalId ON WorldCupPlanner.dbo.Partidos (  EquipoLocalId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Partidos_EquipoVisitanteId ON WorldCupPlanner.dbo.Partidos (  EquipoVisitanteId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Partidos_EstadioId ON WorldCupPlanner.dbo.Partidos (  EstadioId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Partidos_GanadorPorPenalesId ON WorldCupPlanner.dbo.Partidos (  GanadorPorPenalesId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Partidos_GrupoId ON WorldCupPlanner.dbo.Partidos (  GrupoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;