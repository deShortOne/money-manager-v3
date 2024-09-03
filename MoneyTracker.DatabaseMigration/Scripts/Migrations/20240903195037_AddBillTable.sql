CREATE TABLE bill (
    id SERIAL PRIMARY KEY,
    payee VARCHAR(50) NOT NULL,
    amount DECIMAL NOT NULL,
    nextDueDate DATE NOT NULL,
    frequency VARCHAR(50) NOT NULL,
    categoryId SERIAL NOT NULL,
	CONSTRAINT fk_category_with_bill
		FOREIGN KEY(categoryId) 
			REFERENCES Category(id)
);
