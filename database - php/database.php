-- Create the database
CREATE DATABASE miseriorpulse;

-- Use the created database
USE miseriorpulse;

-- Create the `organizations` table
CREATE TABLE organizations (
    organization_id INT AUTO_INCREMENT PRIMARY KEY,
    name_abbreviation VARCHAR(50) NOT NULL
);

-- Create the `users` table
CREATE TABLE users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    organization_id INT NOT NULL,
    position VARCHAR(50),
    FOREIGN KEY (organization_id) REFERENCES organizations(organization_id)
);

-- Create the `user_log` table
CREATE TABLE user_log (
    log_id INT AUTO_INCREMENT PRIMARY KEY,
    date_and_time DATETIME NOT NULL,
    user_id INT NOT NULL,
    log_type VARCHAR(50) NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

-- Create the `attendance_log` table
CREATE TABLE attendance_log (
    attendance_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    date_and_time DATETIME NOT NULL,
    student_id VARCHAR(50) NOT NULL,
    organization_id INT NOT NULL,
    event_name VARCHAR(100) NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    FOREIGN KEY (organization_id) REFERENCES organizations(organization_id)
);
