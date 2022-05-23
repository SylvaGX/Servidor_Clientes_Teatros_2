use master
create database Servidor_Teatro
use Servidor_Teatro
	
create table Localization(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	loc nvarchar(50) NOT NULL,
	lat float not null,
	longi float not null,
);

create table Theater(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	name nvarchar(50) NOT NULL,
	address nvarchar(50) NOT NULL,
	id_localization int FOREIGN KEY REFERENCES Localization(id) NOT NULL,
	contact nvarchar(9) NOT NULL,
	estado int NOT NULL,
	Check(contact like '[9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
);

create table Show(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	name nvarchar(50) NOT NULL,
	sinopse nvarchar(150) NOT NULL,
	id_theater int FOREIGN KEY REFERENCES Theater(id) NOT NULL,
	startDate date NOT NULL,
	endDate date NOT NULL,
	price money NOT NULL,
	estado int NOT NULL
);

create table Session(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	id_show int FOREIGN KEY REFERENCES Show(id) NOT NULL,
	sessionDate date NOT NULL,
	startHour time NOT NULL,
	endHour time NOT NULL,
	avaiable_places int NOT NULL,
	total_places int NOT NULL,
	estado int NOT NULL
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
	date_purchase datetime default getdate() not null,
	compra_lugares int NOT NULL
);

insert into Localization (loc, lat, longi)
values ('Porto', 5, 5);

insert into Users (name, mail, pass, type, fundos, id_localization)
values ('Tiago', 'ola@email.com', '1234', '1', 5, 1);

insert into Users (name, mail, pass, type, fundos, id_localization)
values ('Tomas', 'ola2@email.com', '1234', '2', 0, 1);

insert into Theater (name, address, contact, id_localization, estado)
values ('Rivoli', 'Rua da Capela', '934920498', 1, 1);

insert into Show(name, sinopse, startDate, endDate, price, id_theater, estado)
values ('Harry Potter', 'Kali Linux', '2022-05-23', '2022-05-29', 5, 1, 1);

insert into Show(name, sinopse, startDate, endDate, price, id_theater, estado)
values ('Madagascar', 'OSX', '2022-06-5', '2022-06-16', 8, 1, 1);

insert into Session(sessionDate, startHour, endHour, avaiable_places, total_places, id_show, estado)
values ('2022-05-23', '11:00:00', '12:00:00', 30, 60, 1, 1);

Select * from Theater;
Select * from Session;
Select * from Show;
Select * from Users;
Select * from Purchase;
Select * from Localization;