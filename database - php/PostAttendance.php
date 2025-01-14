<?php
$servername = "localhost";
$username = "root";
$password = "root";
$dbname = "miseriorpulse";

// User inputs
$user_name = $_POST['user_name'];
$student_id = $_POST['student_id'];
$name_abbreviation = $_POST['name_abbreviation']; // new field for organization abbreviation
$event_name = $_POST['event_name'];

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    echo json_encode(["message" => "Connection failed: " . $conn->connect_error]);
    exit();
}

// Get user_id from the users table based on the user_name
$sql = "SELECT user_id FROM users WHERE username = ?";
$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $user_name); // 's' means the parameter is a string
$stmt->execute();
$stmt->store_result();

// Check if the user exists
if ($stmt->num_rows > 0) {
    // Fetch the user_id
    $stmt->bind_result($user_id);
    $stmt->fetch();
    
    // Now get the organization_id from the organizations table based on the name_abbreviation
    $sql = "SELECT organization_id FROM organizations WHERE name_abbreviation = ?";
    $stmt = $conn->prepare($sql);
    $stmt->bind_param("s", $name_abbreviation); // 's' for string
    $stmt->execute();
    $stmt->store_result();

    // Check if the organization exists
    if ($stmt->num_rows > 0) {
        // Fetch the organization_id
        $stmt->bind_result($organization_id);
        $stmt->fetch();

        ob_start(); // Start output buffering
        include('GetDate.php');
        $date_and_time = ob_get_clean(); // Get the content of GetDate.php and clean the buffer

        // Prepare the SQL statement to insert into attendance_log
        $sql = "INSERT INTO attendance_log (user_id, date_and_time, student_id, organization_id, event_name) 
                VALUES (?, ?, ?, ?, ?)";
        
        // Prepare and bind
        $stmt = $conn->prepare($sql);
        $stmt->bind_param("issis", $user_id, $date_and_time, $student_id, $organization_id, $event_name); // "issi" means integer, string, string, integer

        // Execute the statement
        if ($stmt->execute()) {
            echo json_encode(["message" => "Attendance logged successfully."]);
        } else {
            echo json_encode(["message" => "Failed to log attendance."]);
        }
    } else {
        // If no organization is found with the given name_abbreviation
        echo json_encode(["message" => "Organization not found."]);
    }
} else {
    // If no user is found with the given username
    echo json_encode(["message" => "User not found."]);
}

// Close the connection
$stmt->close();
$conn->close();
?>
