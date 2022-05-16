use master
create database Servidor_Teatro
use Servidor_Teatro
	
create table Teatros(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	nome nvarchar(50) NOT NULL,
	morada nvarchar(50) NOT NULL,
	localizacao nvarchar(50) NOT NULL,
	contatos nvarchar(9) NOT NULL,
	Check(contatos like '[9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]')
);

create table Espetaculos(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	nome nvarchar(50) NOT NULL,
	sinopse nvarchar(150) NOT NULL,
	id_teatro int FOREIGN KEY REFERENCES Teatros(id) NOT NULL,
	data_ini date NOT NULL,
	data_fim date NOT NULL,
	preco money NOT NULL
);

create table Sessoes(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	id_espetaculo int FOREIGN KEY REFERENCES Espetaculos(id) NOT NULL,
	data_espect date NOT NULL,
	hora_ini time NOT NULL,
	hora_fim time NOT NULL,
	lugares_disponiveis int NOT NULL,
	lugares_totais int NOT NULL
);

create table Users(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	nome nvarchar(50) NOT NULL,
	pass nvarchar(50) NOT NULL,
	tipo varchar(1) NOT NULL,
	mail nvarchar(50) Not NULL,
	fundos money default 0 NOT NULL,
	CHECK(tipo like '[1-3]')
);

create table Comprar(
	id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	id_espetaculo int FOREIGN KEY REFERENCES Espetaculos(id) NOT NULL,
	id_users int FOREIGN KEY REFERENCES Users(id) NOT NULL,
	referencia nvarchar(20) NOT NULL,
	data_compra date default getdate() not null,
);

Select * from Teatros;
Select * from Sessoes;
Select * from Espetaculos;
Select * from Users;
Select * from Comprar;
