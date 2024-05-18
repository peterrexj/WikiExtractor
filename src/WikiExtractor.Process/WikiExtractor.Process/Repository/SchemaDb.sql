CREATE TABLE [tblVersion] (
	[Id]	INTEGER NOT NULL UNIQUE,
	[Version] TEXT,
	PRIMARY KEY([Id] AUTOINCREMENT)
);

CREATE TABLE [tblMaster] (
	[Id]	INTEGER NOT NULL UNIQUE,
	[Name]	TEXT,
	[Route] TEXT,
	PRIMARY KEY([Id] AUTOINCREMENT)
);

CREATE TABLE [tblMetadata] (
	[Id]		INTEGER NOT NULL UNIQUE,
	[MasterId]	INTEGER,
	[Key]		TEXT,
	[Value]		TEXT,
	[Order]		INTEGER,
	[Type]		TEXT,
	PRIMARY KEY([Id] AUTOINCREMENT)
);

CREATE TABLE [tblParagraphPrimaryContent] (
	[Id]			INTEGER NOT NULL UNIQUE,
	[MasterId]		INTEGER,
	[Content]		TEXT,
	PRIMARY KEY([Id] AUTOINCREMENT)
);

CREATE TABLE [tblParagraphHeader2] (
	[Id]			INTEGER NOT NULL UNIQUE,
	[MasterId]		INTEGER,
	[Header]		TEXT,
	PRIMARY KEY([Id] AUTOINCREMENT)
);

CREATE TABLE [tblParagraphHeader3] (
	[Id]						INTEGER NOT NULL UNIQUE,
	[MasterId]					INTEGER,
	[ParagraphHeader2Id]		INTEGER,
	[Header]					TEXT,
	PRIMARY KEY([Id] AUTOINCREMENT)
);

CREATE TABLE [tblParagraphContent] (
	[Id]						INTEGER NOT NULL UNIQUE,
	[MasterId]					INTEGER,
	[ParagraphHeader2Id]		INTEGER,
	[ParagraphHeader3Id]		INTEGER,
	[Content]					TEXT,
	PRIMARY KEY([Id] AUTOINCREMENT)
);

CREATE TABLE [tblParagraphMain] (
	[Id]						INTEGER NOT NULL UNIQUE,
	[MasterId]					INTEGER,
	[Content]					TEXT,
	PRIMARY KEY([Id] AUTOINCREMENT)
);

CREATE TABLE [tblImages] (
	[Id]	INTEGER NOT NULL UNIQUE,
	[MasterId]		INTEGER,
	[Path]			TEXT,
	[IsPrimary]		INTEGER,
	PRIMARY KEY([Id] AUTOINCREMENT)
);
