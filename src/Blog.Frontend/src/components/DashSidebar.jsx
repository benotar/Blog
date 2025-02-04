import {Sidebar} from "flowbite-react";
import {HiArrowSmRight, HiDocumentText, HiUser} from "react-icons/hi";
import {Link, useLocation} from "react-router-dom";
import {useEffect, useState} from "react";
import {useAppStore} from "../zustand/useAppStore.js";
import {useShallow} from "zustand/react/shallow";
import $axios from "../axios/axios.js";

export default function DashSidebar() {

    const {
        tokens,
        doSignOut,
        currentUser
    } = useAppStore(useShallow((state) => ({
        tokens: state.tokens,
        doSignOut: state.doSignOut,
        currentUser: state.currentUser
    })));
    const location = useLocation();
    const [tab, setTab] = useState("");

    useEffect(() => {
        const urlParams = new URLSearchParams(location.search);
        const tabFromUrl = urlParams.get("tab");
        if (tabFromUrl) {
            setTab(tabFromUrl);
        }
    }, [location.search]);

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
        <Sidebar className="w-full md:w-56">
            <Sidebar.Items>
                <Sidebar.ItemGroup  className="flex flex-col gap-1">
                    <Link to="/dashboard?tab=profile">
                        <Sidebar.Item
                            active={tab === "profile"}
                            icon={HiUser}
                            label={currentUser.role === "Admin" ? "Admin" : "User"}
                            labelColor="dark"
                            as="button"
                        >
                            Profile
                        </Sidebar.Item>
                    </Link>
                    {
                        currentUser.role === "Admin" && (
                            <Link to="/dashboard?tab=posts">
                                <Sidebar.Item
                                    active={tab === "posts"}
                                    icon={HiDocumentText}
                                    as="div"
                                >
                                    Posts
                                </Sidebar.Item>
                            </Link>
                        )
                    }
                    <Sidebar.Item
                        icon={HiArrowSmRight}
                        className="cursor-pointer"
                        onClick={handleSignOut}
                    >
                        Sign out
                    </Sidebar.Item>
                </Sidebar.ItemGroup>
            </Sidebar.Items>
        </Sidebar>
    );
}