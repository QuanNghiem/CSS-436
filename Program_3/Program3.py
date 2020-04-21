import os
import os.path
import boto3
import sys
import botocore


def backup(bucketName, directory, client):
    filesIgnore = 0
    filesUpload = 0
    for path, subdirs, files in os.walk(directory):
        path = path.replace("\\", "/")
        # For every files in the designated directory
        for file in files:
            localPath = os.path.join(path, file)
            keyPath = os.path.relpath(localPath, directory)
            keyPath = keyPath.replace("\\", "/")
            # Check if file that contains the same path and key already exist in Bucket
            try:
                client.Object(bucketName, keyPath).load()
            except botocore.exceptions.ClientError as e:
                if e.response['Error']['Code'] == "404":
                    client.Bucket(bucketName).upload_file(localPath, keyPath)
                    filesUpload = filesUpload + 1
                    print("Uploaded: " + localPath)
            else:
                if int(client.Object(bucketName, keyPath).last_modified.timestamp()) < int(os.path.getmtime(localPath)):
                    client.Bucket(bucketName).upload_file(localPath, keyPath)
                    filesUpload = filesUpload + 1
                    print("Uploaded: " + localPath)
                else:
                    filesIgnore = filesIgnore + 1
                    print("Ignored: " + localPath)
    print("Backup Operation Result")
    print("Files uploaded: " + str(filesUpload))
    print("Files ignored: " + str(filesIgnore))


def download(bucketName, requestedDirectory, destPath, client):
    filesDownload = 0
    filesIgnore = 0
    your_bucket = client.Bucket(bucketName)
    requestedDirectory = requestedDirectory.replace('/', '\\')
    for client_file in your_bucket.objects.all():
        currPath = client_file.key.replace('/', '\\')
        lastIndex = len(requestedDirectory)
        currPath = currPath[0:lastIndex]
        if currPath == requestedDirectory:
            localPath = destPath + "\\" + client_file.key.replace('/', '\\')
            if os.path.exists(localPath):
                if (int(client.Object(bucketName, client_file.key).last_modified.timestamp())) > int(
                        os.path.getmtime(localPath)):
                    file = os.path.join(destPath, client_file.key.replace('/', '\\'))
                    if not os.path.exists(os.path.dirname(file)):
                        os.makedirs(os.path.dirname(file))
                    your_bucket.download_file(client_file.key, file)
                    filesDownload = filesDownload + 1
                    print("Downloaded: " + client_file.key)
                else:
                    filesIgnore = filesIgnore + 1
                    print("Ignored: " + client_file.key)
            else:
                file = os.path.join(destPath, client_file.key.replace('/', '\\'))
                if not os.path.exists(os.path.dirname(file)):
                    os.makedirs(os.path.dirname(file))
                your_bucket.download_file(client_file.key, file)
                filesDownload = filesDownload + 1
                print("Downloaded: " + client_file.key)
    print("Download Operation Result")
    print("Files downloaded: " + str(filesDownload))
    print("Files ignored: " + str(filesIgnore))


def createBucket(bucketName, client):
    if client.Bucket(bucketName).creation_date is None:
        client.create_bucket(CreateBucketConfiguration={'LocationConstraint': "us-west-2"}, Bucket=bucketName)
        return True
    else:
        return False


def main():
    print("AWS S3 Syncing Application")
    print("Download function structure: download bucketName destination folder.")
    print("Example: download mybucket C:\\Users\\ASUS\\Desktop\\Sync\n")
    print("Upload function structure: upload bucketName backup folder.")
    print("Example: upload mybucket C:\\Users\\ASUS\\Desktop\\Upload\n")
    answer = ""
    while answer != "exit" and answer != "[exit]":
        # Getting user inputs
        if len(sys.argv) == 3:
            print("Program received commands from command line.")
            answer = input("Enter your bucket name: ")
            command_list = answer.split(" ")
            if command_list[0] == "":
                print("Invalid argument found.")
                continue
            else:
                command_list.insert(0, sys.argv[1])
                command_list.insert(2, sys.argv[2])
                sys.argv.clear()
        else:
            answer = input("Enter your command: ")
            command_list = answer.split(" ")
            if command_list[0] == "":
                print("Invalid argument found.")
                continue
        # Initiating the arguments
        client = boto3.resource('s3')
        if command_list[0] == "download":
            if len(command_list) == 3:
                if not client.Bucket(command_list[1]).creation_date is None:
                    bucket = client.Bucket(command_list[1])
                    print("All folders and sub-folders in bucket")
                    for file in bucket.objects.all():
                        lastIndex = file.key.rfind("/")
                        print(file.key[0:lastIndex])
                    requestedDirectory = input("Enter your directory: ")
                    if os.path.isdir(command_list[2]):
                        download(command_list[1], requestedDirectory, command_list[2], client)
                        answer = input("\nPress enter to continue. Type [exit] to exit program:")
                    else:
                        answer = input(
                            "Error directory path. Press enter to continue. Type [exit] to exit program:")
                else:
                    answer = input(
                        "No bucket to download from. Press enter to continue. Type [exit] to exit program:")
            else:
                answer = input("Error command. Press enter to continue. Type [exit] to exit program:")
        elif command_list[0] == "upload":
            if len(command_list) == 3:
                if createBucket(command_list[1], client):
                    if os.path.isdir(command_list[2]):
                        backup(command_list[1], command_list[2], client)
                        answer = input("\nPress enter to continue. Type [exit] to exit program:")
                    else:
                        answer = input(
                            "Error directory path. Press enter to continue. Type [exit] to exit program:")
                else:
                    temp = input("Bucket found. Please check if you owned this bucket. Press enter to proceed:")
                    if os.path.isdir(command_list[2]):
                        backup(command_list[1], command_list[2], client)
                        answer = input("\nPress enter to continue. Type [exit] to exit program:")
                    else:
                        answer = input(
                            "Error directory path. Press enter to continue. Type [exit] to exit program:")
            else:
                answer = input("Error command. Press enter to continue. Type [exit] to exit program:")
        else:
            answer = input("Error command. Press enter to continue. Type [exit] to exit program:")


main()
