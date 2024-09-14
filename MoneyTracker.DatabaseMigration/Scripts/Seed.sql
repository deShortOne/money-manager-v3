INSERT INTO Category (name) VALUES
	-- Income
	('Wages & Salary : Net Pay'),
	-- Committed Expenses
	('Bills : Cell Phone'),
	('Bills : Rent'),
	('Groceries'),
	-- Irregular Expenses
	('Hobby'),
    ('Pet Care');
INSERT INTO BudgetGroup (name) VALUES 
	('Income'),
	('Committed Expenses'),
	('Fun'),
	('Irregular Expenses'),
	('Savings & Debt'),
	('Retirement');


INSERT INTO users(name) VALUES
('root');

INSERT INTO account(name, users_id) VALUES
('bank a', 1);

INSERT INTO register (payee, amount, datePaid, category_id, account_id) VALUES
	('Company A', 1800, '2024-08-28', 1, 1),
	('Phone company', 10, '2024-08-01', 2, 1),
	('Landlord A', 500, '2024-08-01', 3, 1),
	('Supermarket', 25, '2024-08-01', 4, 1),
	('Supermarket', 23, '2024-08-08', 4, 1),
	('Supermarket', 27, '2024-08-15', 4, 1),
	('Hobby item', 150, '2024-08-09', 5, 1);

INSERT INTO bill (payee, amount, nextduedate, frequency, category_id, monthday, account_id) VALUES
    ('supermarket a', 23, '2024-09-03', 'Weekly', 4, 3, 1),
    ('company a', 100, '2024-08-30', 'Monthly', 1, 30, 1);

INSERT INTO BudgetCategory VALUES 
	(1, 1, 1, 1800),
	(1, 2, 2, 10),
	(1, 2, 3, 500),
	(1, 2, 4, 100),
	(1, 4, 5, 50);

