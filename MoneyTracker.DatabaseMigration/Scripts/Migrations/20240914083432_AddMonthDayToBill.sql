ALTER TABLE bill 
ADD COLUMN monthday int;

UPDATE bill 
SET monthday = extract('day' from nextduedate);

ALTER TABLE bill
ALTER COLUMN monthday SET NOT NULL;
