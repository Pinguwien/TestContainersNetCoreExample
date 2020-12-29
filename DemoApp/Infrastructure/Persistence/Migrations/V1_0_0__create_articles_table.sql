CREATE TABLE articles (
	articleId serial NOT NULL PRIMARY KEY,
	name TEXT NOT NULL,
	description TEXT,
	price NUMERIC (15,6) NOT NULL,
	created TIMESTAMP NOT NULL
);