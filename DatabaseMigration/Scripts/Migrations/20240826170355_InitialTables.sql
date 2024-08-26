CREATE TABLE BudgetGroup (
	id SERIAL PRIMARY KEY,
	name VARCHAR(50) NOT NULL
);

INSERT INTO BudgetGroup (name) VALUES 
	('Income'),
	('Committed Expenses'),
	('Fun'),
	('Irregular Expenses'),
	('Savings & Debt'),
	('Retirement');

CREATE TABLE Category (
	id SERIAL PRIMARY KEY,
	name VARCHAR(50) NOT NULL
);

INSERT INTO Category (name) VALUES
	-- Income
	('Wages & Salary : Net Pay'),

	-- Committed Expenses
	('Bills : Cell Phone'),
	('Bills : Rent'),
	('Groceries'),
	
	-- Irregular Expenses
	('Hobby');

CREATE TABLE Bills (
	id SERIAL PRIMARY KEY,
	payee VARCHAR(50) NOT NULL,
	amount DECIMAL NOT NULL,
	datePaid TIMESTAMP WITH TIME ZONE NOT NULL,
	category SERIAL NOT NULL,
	CONSTRAINT fk_customer
		FOREIGN KEY(category) 
			REFERENCES Category(id)
);

INSERT INTO Bills (payee, amount, datePaid, category) VALUES
	('Company A', 1800, '2024-08-28', 1),
	('Phone company', 10, '2024-08-01', 2),
	('Landlord A', 500, '2024-08-01', 3),
	('Supermarket', 25, '2024-08-01', 4),
	('Supermarket', 23, '2024-08-08', 4),
	('Supermarket', 27, '2024-08-15', 4),
	('Hobby item', 150, '2024-08-9', 4);