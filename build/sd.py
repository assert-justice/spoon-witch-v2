from sd_utils import Cli
import os

def match_start(line: str, pattern: str) -> tuple[bool, str]:
    if line.startswith(pattern):
        line = line[len(pattern):].strip()
        return (True, line)
    return (False, line)

def get_targets() -> list[str]:
    res: list[str] = []
    with open("./export_presets.cfg") as file:
        lines = file.readlines()
        for line in lines:
            (is_match, s) = match_start(line, "name=")
            if is_match:
                s = s[1:-1]
                res.append(s)
    return res

def get_project_name(cli: Cli) -> str:
    if "project_name" not in cli.data:
        cli.data["project_name"] = "SpoonWitch"
    return cli.data["project_name"]

def get_project_web_name(cli: Cli) -> str:
    if "project_web_name" not in cli.data:
        cli.data["project_web_name"] = "spoon-witch"
    return cli.data["project_web_name"]

def get_user(cli: Cli) -> str:
    if "user" not in cli.data:
        cli.data["user"] = "etmm"
    return cli.data["user"]

def get_extension(cli: Cli) -> str:
    if "file_ext" not in cli.data:
        target = cli.data["target"]
        lookup = {
            "win": "exe",
            "linux": "x86_64",
            "mac": "app"
        }
        cli.data["file_ext"] = lookup[target]
    # Todo: handle lookup error gracefully
    return cli.data["file_ext"]

def get_filename(cli: Cli) -> str:
    if "filename" not in cli.data:
        cli.data["filename"] = f"{get_project_name(cli)}.{get_extension(cli)}"
    return cli.data["filename"]

def get_directory(cli: Cli) -> str:
    if "directory" not in cli.data:
        target = cli.data["target"]
        path = os.path.join("build", "bin", target)
        # make target dir if it doesn't exist
        cli.makedirs(path)
        cli.data["directory"] = path
    return cli.data["directory"]

def get_filepath(cli: Cli) -> str:
    if "filepath" not in cli.data:
        cli.data["filepath"] = os.path.join(get_directory(cli), get_filename(cli))
    return cli.data["filepath"]

def build(cli: Cli):
    target = cli.data["target"]
    # call godot build cli
    command = f"godot --headless --export-release {target} {get_filepath(cli)}"
    # print(command)
    cli.run(command.split(" "))
    pass

def publish(cli: Cli):
    target = cli.data["target"]
    # butler push directory user/game:channel
    command = f"butler push {get_directory(cli)} {get_user(cli)}/{get_project_web_name(cli)}:{target}"
    cli.run(command.split(" "))
    pass

def build_and_publish(cli: Cli):
    build(cli)
    publish(cli)

def main():
    targets = get_targets()
    cli = Cli()
    if not cli.has_args():
        cli.args.extend(targets)
        # build and publish all targets
        for target in targets:
            cli.data.clear()
            cli.data["target"] = target
            build_and_publish(cli)
        return
if __name__ == "__main__":
    main()
