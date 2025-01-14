<?php
$servername = "localhost";
$username = "root";
$password = "root";
$dbname = "miseriorpulse";

// Get username from POST request
$orgName = $_POST["orgName"];

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

// Query to fetch attendance log with username and organization abbreviation
$query = "
    SELECT 
        u.username,
        al.event_name,
        al.student_id
    FROM 
        attendance_log AS al
    INNER JOIN 
        users AS u ON al.user_id = u.user_id
    INNER JOIN 
        organizations AS org ON al.organization_id = org.organization_id
    WHERE 
        org.name_abbreviation = ?";
$stmt = $conn->prepare($query);
$stmt->bind_param("s", $orgName);
$stmt->execute();
$result = $stmt->get_result();

$attendanceData = [];
while ($row = $result->fetch_assoc()) {
    $attendanceData[] = $row;
}

// Return the data as JSON
echo json_encode($attendanceData);

// Close connection
$conn->close();
?>
