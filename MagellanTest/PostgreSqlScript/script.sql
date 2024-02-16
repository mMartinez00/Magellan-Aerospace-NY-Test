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
