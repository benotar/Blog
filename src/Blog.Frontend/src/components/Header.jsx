import {Avatar, Button, Dropdown, Navbar} from "flowbite-react";
import {Link, useLocation} from "react-router-dom";
import {FaMoon, FaSun} from "react-icons/fa";
import {useAppStore} from "../zustand/useAppStore.js";
import $axios from "../axios/axios.js";
import {useShallow} from "zustand/react/shallow";

export default function Header() {

    const {
        tokens,
        doSignOut
    } = useAppStore(useShallow((state) => ({
        tokens: state.tokens,
        doSignOut: state.doSignOut,
    })));
    const path = useLocation().pathname;

    const currentUser = useAppStore((state) => state.currentUser);
    const toggleTheme = useAppStore((state) => state.toggleTheme);
    const theme = useAppStore((state) => state.theme);

    const handleSignOut = async () => {
        try {
            const {data} = await $axios.post("auth/sign-out", {
                refreshToken: tokens.refreshToken
            });

            doSignOut();

        } catch (error) {
            console.log(error.message);
        }
    };

    return (
        <Navbar className="border-b-2">
            <Link to="/" className="self-center whitespace-nowrap text-sm
            sm:text-xl font-semibold dark:text-white">
                <span
                    className="px-2 py-1 bg-gradient-to-r from-indigo-500
                    via-purple-500 to-pink-500 rounded-lg text-white"
                >
                    Benotar_&apos;s
                </span>
                Blog
            </Link>
            <div className="flex gap-2 md:order-2">
                <Button className="w-12 h-10 hidden sm:inline" color="gray" pill
                        onClick={() => toggleTheme()}
                >
                    {theme === "light" ? <FaMoon/> : <FaSun/>}
                </Button>
                {currentUser ? (
                    <Dropdown
                        arrowIcon={false}
                        inline
                        label={
                            <Avatar
                                alt="user"
                                img={currentUser.profilePictureUrl}
                                rounded
                            />
                        }
                    >
                        <Dropdown.Header>
                            <span className="block text-sm">
                                @{currentUser.username}
                            </span>
                            <span className="block text-sm font-medium truncate">
                                {currentUser.email}
                            </span>
                        </Dropdown.Header>
                        <Link to={"/dashboard?tab=profile"}>
                            <Dropdown.Item>Profile</Dropdown.Item>
                        </Link>
                        <Dropdown.Divider/>
                        <Dropdown.Item onClick={handleSignOut}>Sign out</Dropdown.Item>
                    </Dropdown>
                ) : (
                    <Link to="/sign-in">
                        <Button gradientDuoTone="purpleToBlue" outline>
                            Sign In
                        </Button>
                    </Link>
                )}
                <Navbar.Toggle/>
            </div>
            <Navbar.Collapse>
                <Navbar.Link active={path === "/"} as={"div"}>
                    <Link to="/">
                        Home
                    </Link>
                </Navbar.Link>
                <Navbar.Link active={path === "/about"} as={"div"}>
                    <Link to="/about">
                        About
                    </Link>
                </Navbar.Link>
                <Navbar.Link active={path === "/projects"} as={"div"}>
                    <Link to="/projects">
                        Projects
                    </Link>
                </Navbar.Link>
            </Navbar.Collapse>
        </Navbar>
    );
}