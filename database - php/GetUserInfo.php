<?php
$servername = "localhost";
$username = "root";
$password = "root";
$dbname = "miseriorpulse";

// Get userid
$UserID = $_POST["UserID"];

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die(json_encode(["message" => "Connection failed: " . $conn->connect_error]));
}

$sql = "SELECT username, email, organization_id, position FROM users WHERE user_id = ?";
$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $UserID);
$stmt->execute();
$result = $stmt->get_result();

//get data
if ($result->num_rows > 0) {
    $row = $result->fetch_assoc();
    $userName = $row["username"];
    $email = $row["email"];
    $organizationId = $row["organization_id"];
    $position = $row["position"];

    //get org name
    $sql = "SELECT name_abbreviation FROM organizations WHERE organization_id = ?";
    $stmt = $conn->prepare($sql);
    $stmt->bind_param("s", $organizationId);
    $stmt->execute();
    $result = $stmt->get_result();

    if ($result->num_rows > 0) {
        $row = $result->fetch_assoc();
        $organization = $row["name_abbreviation"];

        echo json_encode(["userName" => $userName,
                          "email" => $email,
                          "organization" => $organization,
                          "position" => $position]);
    } else {
        echo json_encode(["message" => "No data found."]);
    }
} else {
    echo json_encode(["message" => "No data found."]);
}

$conn->close();
?>
