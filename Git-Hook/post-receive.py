#!/usr/bin/python

# Imports
import git
import errno, os, stat, shutil
import csv
import subprocess
import logging
import sys
# Imports

# Methods
def get_git_root(path):
    git_repo = git.Repo(path, search_parent_directories=True)
    git_root = git_repo.git.rev_parse("--show-toplevel")
    return git_root
def file_exists(list):
    if len(list) == 0:
        return False
    else:
        return True
def folder_exists(path):
    return os.path.isdir(path)
def find_all(name, path):
    result = []
    for root, dirs, files in os.walk(path):
        if name in files:
            result.append(os.path.join(root, name))
    return result
def remove_readonly(func, path, excinfo):
    os.chmod(path, stat.S_IWRITE)
    func(path)
# Methods

# Customize this part for repository purposes!
repo_path = os.getcwd()
logname = "log.txt"
temp_first = "temp-first"
temp_last = "temp-last"
schema_comparator_folder="Schema-comparator"
create_script = "Create.sql"
update_script = "Update.sql"
first_create_script = "Create.sql"
result_folder = os.path.normpath(os.path.join(os.path.dirname(os.getcwd()),"Schema-comparator","Result"))
conn_str = "ConnStrings.txt"
db_type = "mssql"
# Customize this part for repository purposes!

# Logging of the script
logger = logging.getLogger('test-schemaComparator')
hdlr = logging.FileHandler(os.path.join(result_folder, logname))
formatter = logging.Formatter('%(asctime)s %(levelname)s %(message)s')
hdlr.setFormatter(formatter)
logger.addHandler(hdlr)
logger.setLevel(logging.INFO)
# Logging of the script

logger.info("===============[Start Script]===============")
logger.info("Result folder: {}".format(result_folder))
logger.info("Repo root: {}".format(repo_path))

#### [Step 1] ####
# Clone repository to temporary folder and reset hard to current commit

main_gitpath = repo_path
logger.info("Main git path: {}".format(main_gitpath))
g = git.Git(repo_path)
first_hash = g.execute(["git", "log","-i", "--pretty=format:%H", "--diff-filter=A", "--", "*/" + first_create_script])
commit_hash = g.execute(["git", "rev-parse", "HEAD"])
logger.info("First Create Script Hash:{}".format(first_hash))
logger.info("Current Commit Hash:{}".format(commit_hash))
files = g.execute(["git", "diff-tree","--no-commit-id","--name-status","-r", commit_hash])
logger.info("Files {}".format(files))

#if str(create_script).lower() in str(files).lower() or str(update_script).lower() in str(files).lower():


temp_first_path = os.path.join(repo_path, temp_first)
logger.info("Temp first path: {}".format(temp_first_path))

# Clone Repositories and change work dir and git dir

if not folder_exists(temp_first_path):
    cloned_dir = git.Repo.clone_from(os.path.join(main_gitpath), temp_first_path)
    os.chdir(temp_first_path)
    logger.info("Changing dir to {}.".format(temp_first_path))

    cloned_dir.git.execute(["git", "reset", "--hard", first_hash])
    logger.info("Reset hard to {}".format(first_hash))

else:
    logger.info("Repo {} already exists.".format(temp_first))

    os.chdir(temp_first_path)
    logger.info("Changing dir to {}.".format(temp_first_path))
    cloned_first_repo = git.Repo(temp_first_path).git
    cloned_first_repo.execute(["git", "reset", "--hard", first_hash])
    logger.info("Reset hard to {}".format(first_hash))

first_create_filepath = find_all(first_create_script, os.getcwd())[0]

first_create_test = file_exists(first_create_filepath)
temp_last_path = os.path.join(repo_path, temp_last)
logger.info("Temp Last path: {}".format(temp_last_path))

if not folder_exists(temp_last_path):
     cloned_dir = git.Repo.clone_from(main_gitpath, temp_last_path)
     os.chdir(temp_last_path)
     logger.info("Changing dir to {}.".format(temp_last_path))
     cloned_dir.git.execute(["git", "reset", "--hard", commit_hash])
     logger.info("Reset hard to {}".format(commit_hash))

else:
     logger.info("Repo {} already exists.".format(temp_last_path))
     os.chdir(temp_last_path)
     logger.info("Changing dir to {}.".format(temp_last_path))
     cloned_last_repo = git.Repo(temp_last_path).git
     cloned_last_repo.execute(["git", "reset", "--hard", commit_hash])
     logger.info("Reset hard to {}".format(first_hash))

# Clone Repositories and change work dir and git dir
#### [Step 1] ####

#### [Step 2 - Check if all files are present] ####
create_script_filepath = find_all(create_script, os.getcwd())[0]
create_test = file_exists(create_script_filepath)

update_script_filepath = find_all(update_script, os.getcwd())[0]
update_test = file_exists(update_script_filepath)

connection_str_filepath = find_all(conn_str, os.getcwd())[0]
connection_str_test = file_exists(connection_str_filepath)

logger.info("First Create Script path: {}\nCreate Script path: {}\n Update Script path: {}\n Connection String path: {}"
            .format(first_create_filepath, create_script_filepath, update_script_filepath, connection_str_filepath))

logger.info("First_create_test: {first_test}\nCreate_test:{create_test}\nUpdate test:{update_test}\nConnection test:{conn_str_test}".format(
    first_test=first_create_test, create_test=create_test, update_test=update_test, conn_str_test=connection_str_test))

conn_str_arr = ()
#### [Step 2] ####

#### [Step 3 - Run comparision] ####
if first_create_script and create_script and update_script and connection_str_test:
    with open(connection_str_filepath, 'r') as conn_str_file:
        reader = csv.reader(conn_str_file, delimiter='|')
        conn_str_arr = list(reader)[0]
    logger.info("Input: Connection Strings - {}".format(conn_str_arr))


    os.chdir(os.path.join(repo_path))
    os.chdir(os.path.dirname(os.getcwd()))
    os.chdir(os.path.join(schema_comparator_folder, "ScriptRunner"))
    logger.info("Changing dir to {}".format(os.getcwd()))
    try:
        exit_status_1 = subprocess.call(["DBSchemaComparator.ScriptRunner.exe", conn_str_arr[0], db_type, os.path.normpath(first_create_filepath), os.path.normpath(result_folder)])
        exit_status_2 = subprocess.call(["DBSchemaComparator.ScriptRunner.exe", conn_str_arr[1], db_type, os.path.normpath(create_script_filepath), os.path.normpath(result_folder)])
        exit_status_3 = subprocess.call(["DBSchemaComparator.ScriptRunner.exe", conn_str_arr[0], db_type, os.path.normpath(update_script_filepath), os.path.normpath(result_folder)])
    except Exception as e:
        logger.exception(e)
    os.chdir(os.path.dirname(os.getcwd()))
    os.chdir(os.path.join("SchemaComparator"))
    logger.info("Changing dir to {}".format(os.getcwd()))
    if exit_status_1 != 0 or exit_status_2 != 0 or exit_status_3 != 0:
        logger.info("Script deployment failed!")
    else:
        try:
            exit_status_4 = subprocess.call(
            ["DBSchemaComparator.App.exe", conn_str_arr[0], conn_str_arr[1], db_type, os.path.normpath(result_folder)])
            exit_status_5 = subprocess.call(
            ["DBSchemaComparator.App.exe", conn_str_arr[0], conn_str_arr[2], db_type, os.path.normpath(result_folder)])
        except Exception as e:
            logger.exception(e)
        if exit_status_4 != 0 and exit_status_5 != 0:
            logger.info("Comparison of databases failed!")
            logger.info("Exit status {} of 0 and 1: {} {}".format(exit_status_4, conn_str_arr[0], conn_str_arr[1]))
            logger.info("Exit status {} of 0 and 2: {} {}".format(exit_status_5, conn_str_arr[0], conn_str_arr[2]))
        else:
            logger.info("Comparison of databases successful!")
#### [Step 3] ####

#### [Step 4 - Clean up temporary folders] ####
logger.info("Clean up")
try:
    os.chdir(os.path.join(repo_path))
    os.chdir(os.path.dirname(os.getcwd()))
    os.chdir(os.path.join(schema_comparator_folder, "ScriptRunner"))
    clean_result_1 = subprocess.call(["DBSchemaComparator.ScriptRunner.exe", conn_str_arr[0], db_type, "DELETE", os.path.normpath(result_folder)])
    clean_result_2 = subprocess.call(["DBSchemaComparator.ScriptRunner.exe", conn_str_arr[1], db_type, "DELETE", os.path.normpath(result_folder)])
except Exception as e:
     logger.exception(e)

os.chdir(repo_path)
logger.info("Changing dir to {}".format(os.getcwd()))

shutil.rmtree(temp_first_path, onerror=remove_readonly)
shutil.rmtree(temp_last_path, onerror=remove_readonly)
#### [Step 4] ####

logger.info("Removing {} and {}".format(temp_first_path, temp_last_path))
logger.info("End of a script!")
