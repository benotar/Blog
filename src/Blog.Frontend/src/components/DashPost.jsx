import {useEffect, useState} from "react";
import $axios from "../axios/axios.js";
import {useAppStore} from "../zustand/useAppStore.js";
import {useShallow} from "zustand/react/shallow";
import {Table} from "flowbite-react";
import {Link} from "react-router-dom";

const DashPost = () => {

    const {currentUser} = useAppStore(useShallow((state) => ({
        currentUser: state.currentUser
    })));
    const [showMore, setShowMore] = useState(true);
    const [userPosts, setUserPosts] = useState([]);
    const pageSize = 9;

    useEffect(() => {
        const fetchPosts = async () => {
            try {
                const {data} = await $axios.get(`post/get-posts?userId=${currentUser.id}&page=${1}&pageSize=${pageSize}`);

                if (data.isSucceed) {
                    setUserPosts(data.payload.data.items);

                    if (!data.payload.data.hasNextPage) {
                        setShowMore(false);
                    }
                }

            } catch (error) {
                console.log(error.message);
            }
        }

        if (currentUser.role === "Admin") {
            fetchPosts();
        }
    }, [currentUser.id]);

    const handleShowMore = async () => {

        const nextPageIndex = Math.ceil(userPosts.length / pageSize) + 1;
        try {
            const {data} = await $axios.get(`post/get-posts?userId=${currentUser.id}&page=${nextPageIndex}&pageSize=${pageSize}`);

            if (data.isSucceed) {
                setUserPosts((prev) => [...prev, ...data.payload.data.items]);
                if (!data.payload.data.hasNextPage) {
                    setShowMore(false);
                }
            }
        } catch (error) {
            console.log(error.message);
        }
    }

    console.log(userPosts);

    return (
        <div className="table-auto overflow-x-scroll md:mx-auto p-3 scrollbar
        scrollbar-track-slate-100 scrollbar-thumb-slate-300 dark:scrollbar-track-slate-700 dark:scrollbar-thumb-slate-500">
            {
                currentUser.role === "Admin" && userPosts.length > 0
                    ? (
                        <>
                            <Table
                                hoverable
                                className="shadow-md"
                            >
                                <Table.Head>
                                    <Table.HeadCell>
                                        Date updated
                                    </Table.HeadCell>
                                    <Table.HeadCell>
                                        Post image
                                    </Table.HeadCell>
                                    <Table.HeadCell>
                                        Post title
                                    </Table.HeadCell>
                                    <Table.HeadCell>
                                        Post category
                                    </Table.HeadCell>
                                    <Table.HeadCell>
                                        Delete
                                    </Table.HeadCell>
                                    <Table.HeadCell>
                                        <span>Edit</span>
                                    </Table.HeadCell>
                                </Table.Head>
                                {userPosts.map((post) =>
                                    <Table.Body
                                        className="divide-y"
                                    >
                                        <Table.Row
                                            className="bg-white dark:border-gray-700 dark:bg-gray-800"
                                        >
                                            <Table.Cell>
                                                {new Date(post.updatedAt).toLocaleDateString()}
                                            </Table.Cell>
                                            <Table.Cell>
                                                <Link to={`/post/${post.slug}`}>
                                                    <img
                                                        src={post.imageUrl}
                                                        alt={post.title}
                                                        className="w-20 h-20 object-cover bg-gray-500"
                                                    />
                                                </Link>
                                            </Table.Cell>
                                            <Table.Cell>
                                                <Link
                                                    to={`/post/${post.slug}`}
                                                    className="font-medium text-gray-900 dark:text-white"
                                                >
                                                    {post.title}
                                                </Link>
                                            </Table.Cell>
                                            <Table.Cell>
                                                <Link to={`/post/${post.slug}`}>
                                                    {post.category}
                                                </Link>
                                            </Table.Cell>
                                            <Table.Cell>
                                                <span
                                                    className="font-medium text-red-500 hover:underline cursor-pointer"
                                                >
                                                    Delete
                                                </span>
                                            </Table.Cell>
                                            <Table.Cell>
                                                <Link
                                                    to={`/update-post/${post.id}`}
                                                    className="text-teal-500 hover:underline"
                                                >
                                                    <span>
                                                        Edit
                                                    </span>
                                                </Link>
                                            </Table.Cell>
                                        </Table.Row>
                                    </Table.Body>
                                )}
                            </Table>
                            {showMore && (
                                <button
                                    onClick={handleShowMore}
                                    className="w-full text-teal-500 self-center text-sm py-7"
                                >
                                    Show more
                                </button>
                            )}
                        </>
                    )
                    : (
                        <p>You have no posts yet!</p>
                    )
            }
        </div>
    );
};

export default DashPost;