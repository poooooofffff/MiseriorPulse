<?php
header("Content-Type: application/json");

$servername = "localhost";
$username = "root";
$password = "root";
$dbname = "miseriorpulse";

// User Inputs
$User = htmlspecialchars($_POST["User"]);
$Email = htmlspecialchars($_POST["Email"]);
$Pass = htmlspecialchars($_POST["Pass"]);
$OrgId = htmlspecialchars($_POST["OrgId"]);
$Position = htmlspecialchars($_POST["Position"]);

$hashedPass = password_hash($Pass, PASSWORD_BCRYPT); // Hashing the password

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    echo json_encode(["message" => "Connection failed: " . $conn->connect_error]);
    exit();
}

// Validate email format
if (!filter_var($Email, FILTER_VALIDATE_EMAIL)) {
    echo json_encode(["message" => "Invalid email format."]);
    exit();
}

// Check if username exists
$stmt1 = $conn->prepare("SELECT username FROM users WHERE username = ?");
$stmt1->bind_param("s", $User);
$stmt1->execute();
$result1 = $stmt1->get_result();
if ($result1->num_rows > 0) {
    echo json_encode(["message" => "Username already exists."]);
    exit();
}

// Check if email exists
$stmt2 = $conn->prepare("SELECT email FROM users WHERE email = ?");
$stmt2->bind_param("s", $Email);
$stmt2->execute();
$result2 = $stmt2->get_result();
if ($result2->num_rows > 0) {
    echo json_encode(["message" => "Email is taken."]);
    exit();
}

// Check if OrgId is valid
if ($OrgId === "0") {
    echo json_encode(["message" => "There was an error connecting with our servers. Please try again."]);
    exit();
}

// Insert data securely
$stmt3 = $conn->prepare("INSERT INTO users (username, email, password, organization_id, position) VALUES (?, ?, ?, ?, ?)");
$stmt3->bind_param("sssis", $User, $Email, $hashedPass, $OrgId, $Position);

if ($stmt3->execute()) {
    // Get the inserted user ID
    $userID = $conn->insert_id;

    // Include GetDate.php to get the current date and time
    ob_start(); // Start output buffering
    include('GetDate.php');
    $dateAndTime = ob_get_clean(); // Get the content of GetDate.php and clean the buffer

    // Prepare the SQL query to insert the log
    $sql = "INSERT INTO user_log (date_and_time, user_id, log_type) VALUES (?, ?, ?)";
    $stmt4 = $conn->prepare($sql);

    $logType = "REGISTER";

    // Bind parameters
    $stmt4->bind_param("sis", $dateAndTime, $userID, $logType);

    // Execute the statement
    if ($stmt4->execute()) {
        echo json_encode(["message" => "Successfully Registered.", "userID" => $userID]);
    } else {
        http_response_code(401); // Unauthorized
        echo json_encode(["message" => "Failed to log user activity."]);
    }

    // Close the log statement
    $stmt4->close();
} else {
    echo json_encode(["message" => $stmt3->error]);
}

// Close statements and connection
$stmt1->close();
$stmt2->close();
$stmt3->close();
$conn->close();
?>
