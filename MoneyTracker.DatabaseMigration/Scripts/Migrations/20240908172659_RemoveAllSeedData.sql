ALTER SEQUENCE bills_id_seq RENAME TO register_id_seq;
ALTER SEQUENCE bills_category_seq RENAME TO register_category_seq;

DELETE FROM register;
ALTER SEQUENCE register_id_seq RESTART;
UPDATE register SET id = DEFAULT;

DELETE FROM bill;
ALTER SEQUENCE bill_id_seq RESTART;
UPDATE bill SET id = DEFAULT;

DELETE FROM budgetcategory;
