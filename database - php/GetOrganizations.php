<?php
$servername = "localhost";
$username = "root";
$password = "root";
$dbname = "miseriorpulse";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

$sql = "SELECT name_abbreviation FROM organizations";
$result = $conn->query($sql);

$organizations = array();

if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
        $organizations[] = $row;
    }
    echo json_encode($organizations);
} else {
    echo json_encode([]);
}

$conn->close();
?>
