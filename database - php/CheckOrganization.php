<?php
$servername = "localhost";
$username = "root";
$password = "root";
$dbname = "miseriorpulse";

// User Input
$OrgName = $_POST["OrgName"];
// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

$sql = "SELECT organization_id FROM organizations WHERE name_abbreviation = ?";
$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $OrgName);

$stmt->execute();
$result = $stmt->get_result();

if ($result->num_rows > 0) {
    $row = $result->fetch_assoc();
    echo $row["organization_id"];
} else {
    echo "Unable to identify organization.";
}

$conn->close();
?>