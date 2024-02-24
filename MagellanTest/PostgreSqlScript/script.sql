-- Create database
CREATE DATABASE "Part";

-- Connect to Part database
\c Part;

-- Create table "item"
CREATE TABLE item (
    id          SERIAL      NOT NULL    PRIMARY KEY,
    item_name   VARCHAR(50) NOT NULL,
    parent_item INT         NULL        REFERENCES  item(id),
    cost        INT         NOT NULL,
    req_date    DATE        NOT NULL
);

-- Insert data into "item" table
INSERT INTO item (item_name, parent_item, cost, req_date)
VALUES ('Item1', NULL, 500, '2024-02-20'),
('Sub1', 1, 200, '2024-02-10'),
('Sub2', 1, 300, '2024-01-05'),
('Sub3', 2, 300, '2024-01-02'),
('Sub4', 2, 400, '2024-01-02'),
('Item2', NULL, 600, '2024-03-15'),
('Sub1', 6, 200, '2024-02-25');


CREATE FUNCTION Get_Total_Cost(item_name_input VARCHAR(50))
RETURNS INT AS $$ 
DECLARE 
    total_cost INT; 
BEGIN 
    IF (SELECT parent_item FROM item WHERE item_name = item_name_input) IS NULL THEN
         WITH RECURSIVE my_func(id, item_name, parent_item, cost) AS (
            SELECT id, item_name, parent_item, cost 
            FROM item 
            WHERE item_name = item_name_input
            
            UNION ALL
            
            SELECT i.id, i.item_name, i.parent_item, i.cost 
            FROM item i
            JOIN my_func mf ON i.parent_item = mf.id
        )
        SELECT SUM(cost) INTO total_cost FROM my_func; 
        RETURN total_cost;
    ELSE 
        RETURN total_cost;
    END IF;
END; 
$$ LANGUAGE plpgsql;

SELECT Get_Total_Cost('Sub2');