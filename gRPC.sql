use master
create database Servidor_Teatro
use Servidor_Teatro
	
create table Localization(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	localization nvarchar(50) NOT NULL,
	lat float not null,
	longi float not null,
);

create table Theater(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	name nvarchar(50) NOT NULL,
	address nvarchar(50) NOT NULL,
	id_localization int FOREIGN KEY REFERENCES Localization(id) NOT NULL,
	contact nvarchar(9) NOT NULL,
	Check(contact like '[9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
);

create table Show(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	name nvarchar(50) NOT NULL,
	sinopse nvarchar(150) NOT NULL,
	id_theater int FOREIGN KEY REFERENCES Theater(id) NOT NULL,
	startDate date NOT NULL,
	endDate date NOT NULL,
	price money NOT NULL
);

create table Session(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	id_show int FOREIGN KEY REFERENCES Show(id) NOT NULL,
	sessionDate date NOT NULL,
	startHour time NOT NULL,
	endHour time NOT NULL,
	avaiable_places int NOT NULL,
	total_places int NOT NULL
);

create table Users(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	name nvarchar(50) NOT NULL,
	pass nvarchar(50) NOT NULL,
	type varchar(1) NOT NULL,
	mail nvarchar(50) Not NULL,
	id_localization int FOREIGN KEY REFERENCES Localization(id) not null,
	fundos money default 0 NOT NULL,
	CHECK(type like '[1-3]')
);

create table Purchase(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	id_session int FOREIGN KEY REFERENCES Session(id) NOT NULL,
	id_users int FOREIGN KEY REFERENCES Users(id) NOT NULL,
	reference nvarchar(20) NOT NULL,
	date_purchase date default getdate() not null,
	compra_lugares int NOT NULL
);


Select * from Theater;
Select * from Session;
Select * from Show;
Select * from Users;
Select * from Purchase;
Select * from Localization;