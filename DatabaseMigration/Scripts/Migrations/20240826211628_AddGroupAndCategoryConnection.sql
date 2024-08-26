ALTER TABLE Bills
	RENAME TO Bill;

ALTER TABLE Bill
	RENAME COLUMN category TO category_id;

CREATE TABLE BudgetCategory (
	budget_group_id SERIAL NOT NULL,
	category_id SERIAL NOT NULL,
	planned DECIMAL NOT NULL,
	
	CONSTRAINT fk_budget_group
		FOREIGN KEY(budget_group_id) 
			REFERENCES BudgetGroup(id),
	CONSTRAINT fk_category
		FOREIGN KEY(category_id) 
			REFERENCES Category(id)
);

INSERT INTO BudgetCategory VALUES 
	(1, 1, 1800),
	(2, 2, 10),
	(2, 3, 500),
	(2, 4, 100),
	(4, 5, 50);

UPDATE Bill
	SET category_id = 5
	WHERE payee = 'Hobby item';
