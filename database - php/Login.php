<?php
$servername = "localhost";
$username = "root";
$password = "root";
$dbname = "miseriorpulse";

// User Inputs
$LoginUser = $_POST["LoginUserOrEmail"];
$LoginPass = $_POST["LoginPass"];

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
    // Return error in JSON format
    echo json_encode(["message" => "There was a problem connecting to our servers."]);
    exit();
}

// Prepare the SQL statement to prevent SQL injection
$sql = "SELECT user_id, password FROM users WHERE username = ? OR email = ?";
$stmt = $conn->prepare($sql);

// Bind parameters
$stmt->bind_param("ss", $LoginUser, $LoginUser); // 'ss' for two string parameters

// Execute the statement
$stmt->execute();

// Get the result
$result = $stmt->get_result();

// Check if the username/email exists
if ($result->num_rows > 0) {
    $row = $result->fetch_assoc();
    $hashedPassword = $row["password"];
    $userID = $row["user_id"];

    // Verify the password
    if (password_verify($LoginPass, $hashedPassword)) {
        // Include GetDate.php to get the current date and time
        ob_start(); // Start output buffering
        include('GetDate.php');
        $dateAndTime = ob_get_clean(); // Get the content of GetDate.php and clean the buffer

        // Prepare the SQL query to insert the log
        $sql = "INSERT INTO user_log (date_and_time, user_id, log_type) VALUES (?, ?, ?)";
        $stmt = $conn->prepare($sql);

        $logType = "LOGIN";

        // Bind parameters
        $stmt->bind_param("sis", $dateAndTime, $userID, $logType); // 'sis' for string, integer, string

        // Execute the statement
        if ($stmt->execute()) {
            echo json_encode(["message" => "Successfully Logged in.", "userID" => $userID]);
        } else {
            http_response_code(401); // Unauthorized
            echo json_encode(["message" => "Failed to log user activity."]);
        }
    } else {
        // Return incorrect login error in JSON format
        http_response_code(401); // Unauthorized
        echo json_encode(["message" => "Incorrect username/email or password."]);
    }
} else {
    // Return incorrect login error in JSON format
    http_response_code(401); // Unauthorized
    echo json_encode(["message" => "Incorrect username/email or password."]);
}

// Close the connection
$stmt->close();
$conn->close();
?>
